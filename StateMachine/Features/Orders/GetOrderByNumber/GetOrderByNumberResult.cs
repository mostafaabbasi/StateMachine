using StateMachine.Domain;

namespace StateMachine.Features.Orders.GetOrderByNumber;

public sealed record GetOrderByNumberResult(
    int Id,
    string OrderNumber,
    string CustomerEmail,
    decimal Amount,
    OrderState State,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    bool PaymentProcessed,
    bool InventoryReserved,
    bool Shipped,
    int PaymentRetries,
    OrderState[] AllowedTransitions);
