using MediatR;

namespace StateMachine.Features.Orders.GetOrderStates;

public record GetOrderStatesQuery() : IRequest<GetOrderStatesResult>;