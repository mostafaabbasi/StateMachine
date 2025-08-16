using StateMachine.Domain;

namespace StateMachine.Features.Orders.CreateOrder;

public sealed record CreateOrderResult(
    int Id,
    string OrderNumber,
    string CustomerEmail,
    decimal Amount,
    OrderState State,
    DateTime CreatedAt);