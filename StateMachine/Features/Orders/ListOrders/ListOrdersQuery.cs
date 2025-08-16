using MediatR;

namespace StateMachine.Features.Orders.ListOrders;

public sealed record ListOrdersQuery(int Page = 1, int PageSize = 20) : IRequest<ListOrdersResult>;
