using StateMachine.Domain;

namespace StateMachine.Features.Orders.ListOrders;

public sealed record OrderSummary(
    int Id,
    string OrderNumber,
    string CustomerEmail,
    decimal Amount,
    OrderState State,
    DateTime CreatedAt,
    OrderState[] AllowedTransitions);
