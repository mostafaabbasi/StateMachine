namespace StateMachine.Features.Orders.ListOrders;

public sealed record ListOrdersResult(
    OrderSummary[] Orders,
    int TotalCount,
    int Page,
    int PageSize);
