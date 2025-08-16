using System.Text.Json.Serialization;

namespace StateMachine.Domain;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OrderState
{
    Pending,
    ProcessingPayment,
    PaymentFailed,
    Confirmed,
    Shipped,
    Delivered,
    Cancelled,
    Refunded
}