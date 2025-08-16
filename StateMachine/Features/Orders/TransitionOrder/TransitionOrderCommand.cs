using MediatR;
using StateMachine.Domain;

namespace StateMachine.Features.Orders.TransitionOrder;

public sealed record TransitionOrderCommand(
    int OrderId,
    OrderState TargetState,
    string? Notes = null) : IRequest<TransitionOrderResult>;