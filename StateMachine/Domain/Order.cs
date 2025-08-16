namespace StateMachine.Domain;

public class Order
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string CustomerEmail { get; set; } = string.Empty;
    public OrderState State { get; set; } = OrderState.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool PaymentProcessed { get; set; }
    public bool InventoryReserved { get; set; }
    public bool Shipped { get; set; }
    public int PaymentRetries { get; set; } = 0;
    public List<OrderStateHistory> StateHistory { get; set; } = [];   
}