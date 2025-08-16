using MediatR;
using Microsoft.EntityFrameworkCore;
using StateMachine.Infrastructure.Data;

namespace StateMachine.Features.Orders.GetOrderHistory;

internal sealed class GetOrderHistoryHandler(OrderDbContext context) : IRequestHandler<GetOrderHistoryQuery, GetOrderHistoryResult>
{
    public async Task<GetOrderHistoryResult> Handle(GetOrderHistoryQuery request, CancellationToken cancellationToken)
    {
        var history = await context.OrderStateHistory
            .Where(h => h.OrderId == request.OrderId)
            .OrderBy(h => h.Timestamp)
            .Select(h => new OrderHistoryEntry(h.Id, h.FromState, h.ToState, h.Timestamp, h.Duration, h.Notes))
            .ToArrayAsync(cancellationToken);

        return new GetOrderHistoryResult(history);
    }
}