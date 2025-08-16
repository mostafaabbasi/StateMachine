using StateMachine.Domain;

namespace StateMachine.Features.Orders.GetOrderHistory;

public sealed record OrderHistoryEntry(
    int Id,
    OrderState FromState,
    OrderState ToState,
    DateTime Timestamp,
    TimeSpan Duration,
    string? Notes);
