using MediatR;

namespace StateMachine.Features.Orders.GetOrderHistory;

public sealed record GetOrderHistoryQuery(int OrderId) : IRequest<GetOrderHistoryResult>;