using MediatR;
using Microsoft.EntityFrameworkCore;
using StateMachine.Infrastructure.Data;
using StateMachine.Infrastructure.StateMachine;

namespace StateMachine.Features.Orders.GetOrderByNumber;

internal sealed class GetOrderByNumberHandler(OrderDbContext context, IOrderStateMachineFactory stateMachineFactory) : IRequestHandler<GetOrderByNumberQuery, GetOrderByNumberResult?>
{
    public async Task<GetOrderByNumberResult?> Handle(GetOrderByNumberQuery request, CancellationToken cancellationToken)
    {
        var order = await context.Orders.FirstOrDefaultAsync(o => o.OrderNumber == request.OrderNumber, cancellationToken);
        if (order == null) return null;

        var stateMachine = stateMachineFactory.Create(order);
        var allowedTransitions = stateMachine.GetAllowedTransitions(order.State).ToArray();

        return new GetOrderByNumberResult(order.Id,
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
