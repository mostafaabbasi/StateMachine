using MediatR;
using StateMachine.Domain;
using StateMachine.Infrastructure.Data;

namespace StateMachine.Features.Orders.CreateOrder;

internal sealed class CreateOrderHandler(OrderDbContext context, ILogger<CreateOrderHandler> logger) : IRequestHandler<CreateOrderCommand, CreateOrderResult>
{
    private readonly OrderDbContext _context = context;
    private readonly ILogger<CreateOrderHandler> _logger = logger;

    public async Task<CreateOrderResult> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = new Order
        {
            OrderNumber = GenerateOrderNumber(),
            CustomerEmail = request.CustomerEmail,
            Amount = request.Amount,
            State = OrderState.Pending
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created order {OrderNumber} for {CustomerEmail}", order.OrderNumber, order.CustomerEmail);

        return new CreateOrderResult(order.Id, order.OrderNumber, order.CustomerEmail, order.Amount, order.State, order.CreatedAt);
    }

    private static string GenerateOrderNumber() =>
        $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(1000, 9999)}";
}