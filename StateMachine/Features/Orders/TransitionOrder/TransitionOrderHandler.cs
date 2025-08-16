using MediatR;
using Microsoft.EntityFrameworkCore;
using StateMachine.Domain;
using StateMachine.Infrastructure.Data;
using StateMachine.Infrastructure.StateMachine;

namespace StateMachine.Features.Orders.TransitionOrder;

internal sealed class TransitionOrderHandler(OrderDbContext context, IOrderStateMachineFactory stateMachineFactory, ILogger<TransitionOrderHandler> logger) : IRequestHandler<TransitionOrderCommand, TransitionOrderResult>
{
    public async Task<TransitionOrderResult> Handle(TransitionOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await context.Orders.Include(o => o.StateHistory)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

        if (order == null)
        {
            return new TransitionOrderResult(false, null, OrderState.Pending, "Order not found", TimeSpan.Zero, DateTime.UtcNow);
        }

        var stateMachine = stateMachineFactory.Create(order);
        var result = await stateMachine.TransitionToAsync(request.TargetState, cancellationToken);

        if (result.Success)
        {
            order.State = result.CurrentState;
            order.UpdatedAt = DateTime.UtcNow;

            var historyEntry = new OrderStateHistory
            {
                OrderId = order.Id,
                FromState = result.PreviousState ?? order.State,
                ToState = result.CurrentState,
                Duration = result.Duration,
                Notes = request.Notes
            };

            context.OrderStateHistory.Add(historyEntry);
            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Order {OrderNumber} transitioned from {FromState} to {ToState}", 
                order.OrderNumber, result.PreviousState, result.CurrentState);
        }

        return new TransitionOrderResult(result.Success,
                                         result.PreviousState,
                                         result.CurrentState,
                                         result.ErrorMessage,
                                         result.Duration,
                                         result.Timestamp);
    }
}