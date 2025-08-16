using MediatR;

namespace StateMachine.Features.Orders.GetOrderByNumber;

public sealed record GetOrderByNumberQuery(string OrderNumber) : IRequest<GetOrderByNumberResult?>;
