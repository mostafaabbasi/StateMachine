using StateMachine.Domain;

namespace StateMachine.Features.Orders.TransitionOrder;

public sealed record TransitionOrderResult(
    bool Success,
    OrderState? PreviousState,
    OrderState CurrentState,
    string? ErrorMessage,
    TimeSpan Duration,
    DateTime Timestamp);
