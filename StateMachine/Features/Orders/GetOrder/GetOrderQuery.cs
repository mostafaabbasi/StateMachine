using MediatR;

namespace StateMachine.Features.Orders.GetOrder;

public sealed record GetOrderQuery(int Id) : IRequest<GetOrderResult?>;
