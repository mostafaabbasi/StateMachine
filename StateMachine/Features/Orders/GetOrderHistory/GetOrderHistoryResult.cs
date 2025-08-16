namespace StateMachine.Features.Orders.GetOrderHistory;

public sealed record GetOrderHistoryResult(OrderHistoryEntry[] History);
