using StateMachine.Domain;
using StateMachine.Kernel;

namespace StateMachine.Infrastructure.StateMachine;

public class OrderStateMachineFactory(ILogger<StateMachine<OrderState, Order>> logger) : IOrderStateMachineFactory
{
    private const int MaxPaymentRetries = 3;

    public IStateMachine<OrderState, Order> Create(Order order)
    {
        var stateMachine = new StateMachine<OrderState, Order>(order.State, order, logger);

        stateMachine
            .ConfigureState(OrderState.ProcessingPayment, config => config
                .OnEnterAsync(async (ctx, ct) => {
                    await Task.Delay(500, ct);
                    var random = new Random();
                    if (random.NextDouble() > 0.2 || ctx.PaymentRetries >= MaxPaymentRetries)
                    {
                        ctx.PaymentProcessed = true;
                    }
                    else
                    {
                        ctx.PaymentRetries++;
                    }
                })
                .CanTransitionToWhen((targetState, ctx) => targetState switch
                {
                    OrderState.Confirmed => ctx.PaymentProcessed,
                    OrderState.PaymentFailed => !ctx.PaymentProcessed,
                    OrderState.Cancelled => true,
                    _ => false
                }))

            .ConfigureState(OrderState.Confirmed, config => config
                .OnEnterAsync(async (ctx, ct) => {
                    await Task.Delay(300, ct);
                    ctx.InventoryReserved = true;
                }))

            .ConfigureState(OrderState.Shipped, config => config
                .OnEnterAsync(async (ctx, ct) => {
                    await Task.Delay(100, ct);
                    ctx.Shipped = true;
                }));

        stateMachine
            .AllowTransitions(OrderState.Pending, OrderState.ProcessingPayment, OrderState.Cancelled)
            .AllowTransitions(OrderState.ProcessingPayment, OrderState.Confirmed, OrderState.PaymentFailed, OrderState.Cancelled)
            .AllowTransitions(OrderState.PaymentFailed, OrderState.ProcessingPayment, OrderState.Cancelled)
            .AllowTransitions(OrderState.Confirmed, OrderState.Shipped, OrderState.Cancelled)
            .AllowTransitions(OrderState.Shipped, OrderState.Delivered, OrderState.Cancelled)
            .AllowTransitions(OrderState.Delivered, OrderState.Refunded)
            .AllowTransition(OrderState.Cancelled, OrderState.Refunded);

        return stateMachine;
    }
}