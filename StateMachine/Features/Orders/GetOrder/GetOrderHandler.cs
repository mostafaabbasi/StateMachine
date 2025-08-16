using MediatR;
using StateMachine.Infrastructure.Data;
using StateMachine.Infrastructure.StateMachine;

namespace StateMachine.Features.Orders.GetOrder;

internal sealed class GetOrderHandler(OrderDbContext context, IOrderStateMachineFactory stateMachineFactory) : IRequestHandler<GetOrderQuery, GetOrderResult?>
{
    public async Task<GetOrderResult?> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        var order = await context.Orders.FindAsync([request.Id], cancellationToken);
        if (order == null) return null;

        var stateMachine = stateMachineFactory.Create(order);
        var allowedTransitions = stateMachine.GetAllowedTransitions(order.State).ToArray();

        return new GetOrderResult(order.Id,
                                  order.OrderNumber,
                                  order.CustomerEmail,
                                  order.Amount,
                                  order.State,
                                  order.CreatedAt,
                                  order.UpdatedAt,
                                  order.PaymentProcessed,
                                  order.InventoryReserved,
                                  order.Shipped,
                                  order.PaymentRetries,
                                  allowedTransitions);
    }
}