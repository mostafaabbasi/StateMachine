using StateMachine.Domain;

namespace StateMachine.Features.Orders.GetOrder;

public sealed record GetOrderResult(
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