using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using StateMachine.Domain;
using StateMachine.Infrastructure.Data;
using StateMachine.Infrastructure.StateMachine;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseInMemoryDatabase("OrdersDb"));

builder.Services.AddScoped<IOrderStateMachineFactory, OrderStateMachineFactory>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Order State Machine API - Vertical Slice Architecture",
        Version = "v1",
        Description = "A demonstration of Vertical Slice Architecture with State Machine pattern"
    });
    c.EnableAnnotations();
});

builder.Services.AddLogging();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Order State Machine API v1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at root
        c.DocumentTitle = "Order State Machine API";
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    context.Database.EnsureCreated();
    SeedSampleData(context);
}

app.Run();

static void SeedSampleData(OrderDbContext context)
{
    if (context.Orders.Any()) return;

    var orders = new List<Order>
        {
            new() {
                OrderNumber = "ORD-20241201-1001",
                CustomerEmail = "john.doe@example.com",
                Amount = 99.99m,
                State = OrderState.Pending,
                CreatedAt = DateTime.UtcNow.AddHours(-2)
            },
            new() {
                OrderNumber = "ORD-20241201-1002",
                CustomerEmail = "jane.smith@example.com",
                Amount = 149.50m,
                State = OrderState.Confirmed,
                CreatedAt = DateTime.UtcNow.AddHours(-1),
                PaymentProcessed = true,
                InventoryReserved = true
            },
            new() {
                OrderNumber = "ORD-20241201-1003",
                CustomerEmail = "bob.wilson@example.com",
                Amount = 75.25m,
                State = OrderState.Shipped,
                CreatedAt = DateTime.UtcNow.AddMinutes(-30),
                PaymentProcessed = true,
                InventoryReserved = true,
                Shipped = true
            },
            new() {
                OrderNumber = "ORD-20241201-1004",
                CustomerEmail = "alice.brown@example.com",
                Amount = 199.99m,
                State = OrderState.PaymentFailed,
                CreatedAt = DateTime.UtcNow.AddMinutes(-15),
                PaymentRetries = 2
            }
        };

    context.Orders.AddRange(orders);
    context.SaveChanges();

    var historyEntries = new List<OrderStateHistory>
        {
            new() {
                OrderId = 2,
                FromState = OrderState.Pending,
                ToState = OrderState.ProcessingPayment,
                Timestamp = DateTime.UtcNow.AddMinutes(-50),
                Duration = TimeSpan.FromMilliseconds(450),
                Notes = "Starting payment processing"
            },
            new() {
                OrderId = 2,
                FromState = OrderState.ProcessingPayment,
                ToState = OrderState.Confirmed,
                Timestamp = DateTime.UtcNow.AddMinutes(-49),
                Duration = TimeSpan.FromMilliseconds(850),
                Notes = "Payment successful, inventory reserved"
            },
            new() {
                OrderId = 3,
                FromState = OrderState.Pending,
                ToState = OrderState.ProcessingPayment,
                Timestamp = DateTime.UtcNow.AddMinutes(-25),
                Duration = TimeSpan.FromMilliseconds(320),
                Notes = "Processing payment"
            },
            new() {
                OrderId = 3,
                FromState = OrderState.ProcessingPayment,
                ToState = OrderState.Confirmed,
                Timestamp = DateTime.UtcNow.AddMinutes(-24),
                Duration = TimeSpan.FromMilliseconds(680),
                Notes = "Payment confirmed"
            },
            new() {
                OrderId = 3,
                FromState = OrderState.Confirmed,
                ToState = OrderState.Shipped,
                Timestamp = DateTime.UtcNow.AddMinutes(-20),
                Duration = TimeSpan.FromMilliseconds(150),
                Notes = "Order shipped via express delivery"
            },
            new() {
                OrderId = 4,
                FromState = OrderState.Pending,
                ToState = OrderState.ProcessingPayment,
                Timestamp = DateTime.UtcNow.AddMinutes(-12),
                Duration = TimeSpan.FromMilliseconds(520),
                Notes = "First payment attempt"
            },
            new() {
                OrderId = 4,
                FromState = OrderState.ProcessingPayment,
                ToState = OrderState.PaymentFailed,
                Timestamp = DateTime.UtcNow.AddMinutes(-11),
                Duration = TimeSpan.FromMilliseconds(200),
                Notes = "Payment failed - insufficient funds"
            }
        };

    context.OrderStateHistory.AddRange(historyEntries);
    context.SaveChanges();
}