using StateMachine.Domain;

namespace StateMachine.Domain;

public class OrderStateHistory
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public OrderState FromState { get; set; }
    public OrderState ToState { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public TimeSpan Duration { get; set; }
    public string? Notes { get; set; }
    public Order Order { get; set; } = null!;
}