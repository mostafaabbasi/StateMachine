using MediatR;
using Microsoft.EntityFrameworkCore;
using StateMachine.Infrastructure.Data;
using StateMachine.Infrastructure.StateMachine;

namespace StateMachine.Features.Orders.ListOrders;

internal sealed class ListOrdersHandler(OrderDbContext context, IOrderStateMachineFactory stateMachineFactory) : IRequestHandler<ListOrdersQuery, ListOrdersResult>
{
    public async Task<ListOrdersResult> Handle(ListOrdersQuery request, CancellationToken cancellationToken)
    {
        var totalCount = await context.Orders.CountAsync(cancellationToken);
        
        var orders = await context.Orders
            .OrderByDescending(o => o.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var orderSummaries = orders.Select(order =>
        {
            var stateMachine = stateMachineFactory.Create(order);
            var allowedTransitions = stateMachine.GetAllowedTransitions(order.State).ToArray();
            
            return new OrderSummary(order.Id, order.OrderNumber, order.CustomerEmail, 
                order.Amount, order.State, order.CreatedAt, allowedTransitions);
        }).ToArray();

        return new ListOrdersResult(orderSummaries, totalCount, request.Page, request.PageSize);
    }
}