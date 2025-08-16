using System.ComponentModel.DataAnnotations;
using MediatR;

namespace StateMachine.Features.Orders.CreateOrder;

public sealed record CreateOrderCommand(
    [Required] string CustomerEmail,
    [Required, Range(0.01, 10000)] decimal Amount) : IRequest<CreateOrderResult>;
