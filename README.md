# Order State Machine API

A demonstration project showcasing **Vertical Slice Architecture** combined with the **State Machine pattern** for managing order lifecycles in an e-commerce system.

## 🏗️ Architecture Overview

This project implements Vertical Slice Architecture, where each feature is self-contained with its own:
- Command/Query handlers
- DTOs and results
- Business logic
- Database interactions

Combined with a robust State Machine implementation for managing order state transitions.

## 🎯 Features

- **Order Management**: Create, retrieve, and list orders
- **State Transitions**: Manage order lifecycle with validation
- **State History**: Track all state changes with timestamps
- **Concurrent Safety**: Thread-safe state transitions
- **Custom Business Rules**: Configurable state transition logic
- **API Documentation**: Swagger/OpenAPI integration

## 📊 Order States

The system manages the following order states:

```
Pending → ProcessingPayment → Confirmed → Shipped → Delivered
    ↓           ↓               ↓          ↓
Cancelled   PaymentFailed   Cancelled  Cancelled
    ↓           ↓               ↓          ↓
Refunded    Cancelled       Refunded   Refunded
            ↓
        ProcessingPayment (retry)
```

### State Descriptions

- **Pending**: Order created, awaiting payment processing
- **ProcessingPayment**: Payment is being processed (with retry logic)
- **PaymentFailed**: Payment failed after maximum retries
- **Confirmed**: Payment successful, inventory reserved
- **Shipped**: Order has been shipped to customer
- **Delivered**: Order successfully delivered
- **Cancelled**: Order cancelled (from various states)
- **Refunded**: Refund processed for delivered/cancelled orders

## 🚀 Getting Started

### Prerequisites

- .NET 9.0 SDK
- Visual Studio 2022 or VS Code

### Running the Application

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd StateMachine
   ```

2. **Run the application**
   ```bash
   dotnet run
   ```

3. **Access Swagger UI**
   Navigate to `https://localhost:7196` or `http://localhost:5158`

The application uses an in-memory database with sample data pre-loaded.

## 📡 API Endpoints

### Orders Management

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/orders` | List all orders (paginated) |
| `GET` | `/api/orders/{id}` | Get order by ID |
| `GET` | `/api/orders/by-number/{orderNumber}` | Get order by order number |
| `POST` | `/api/orders` | Create new order |
| `POST` | `/api/orders/{id}/transition` | Transition order state |
| `GET` | `/api/orders/{id}/history` | Get order state history |
| `GET` | `/api/orders/states` | Get all available states |

### Sample API Calls

**Create Order:**
```bash
curl -X POST "https://localhost:7196/api/orders" \
  -H "Content-Type: application/json" \
  -d '{
    "customerEmail": "customer@example.com",
    "amount": 99.99
  }'
```

**Transition Order State:**
```bash
curl -X POST "https://localhost:7196/api/orders/1/transition" \
  -H "Content-Type: application/json" \
  -d '{
    "targetState": "ProcessingPayment",
    "notes": "Starting payment processing"
  }'
```

## 🏛️ Project Structure

```
StateMachine/
├── API/
│   ├── Controllers/           # API Controllers
│   └── DTOs/                 # Data Transfer Objects
├── Domain/                   # Domain Models
│   ├── Order.cs
│   ├── OrderState.cs
│   └── OrderStateHistory.cs
├── Features/                 # Vertical Slices
│   └── Orders/
│       ├── CreateOrder/      # Create order feature
│       ├── GetOrder/         # Get order feature
│       ├── ListOrders/       # List orders feature
│       ├── TransitionOrder/  # State transition feature
│       └── ...
├── Infrastructure/
│   ├── Data/                 # Database context
│   └── StateMachine/         # State machine implementation
├── Kernel/                   # Core state machine framework
└── Program.cs               # Application startup
```

## 🔧 State Machine Configuration

The state machine is configured with business rules in `OrderStateMachineFactory`:

```csharp
stateMachine
    .ConfigureState(OrderState.ProcessingPayment, config => config
        .OnEnterAsync(async (ctx, ct) => {
            // Simulate payment processing
            await ProcessPaymentAsync(ctx, ct);
        })
        .CanTransitionToWhen((targetState, ctx) => targetState switch
        {
            OrderState.Confirmed => ctx.PaymentProcessed,
            OrderState.PaymentFailed => !ctx.PaymentProcessed,
            OrderState.Cancelled => true,
            _ => false
        }))
```

## 🎲 Business Logic Features

### Payment Processing
- Simulates payment processing with 80% success rate
- Implements retry logic (max 3 attempts)
- Automatic state transition based on payment result

### Inventory Management
- Automatically reserves inventory on order confirmation
- Tracks inventory reservation status

### Shipping Tracking
- Updates shipping status on state transition
- Maintains shipping timestamps

## 🧪 Sample Data

The application includes pre-loaded sample data:

1. **Pending Order**: `ORD-20241201-1001` - Ready for payment processing
2. **Confirmed Order**: `ORD-20241201-1002` - Payment processed, ready to ship
3. **Shipped Order**: `ORD-20241201-1003` - Currently shipped
4. **Failed Payment**: `ORD-20241201-1004` - Payment failed, can retry

## 🔍 Key Design Patterns

### Vertical Slice Architecture
- Each feature is self-contained
- Minimal coupling between features
- Clear separation of concerns
- Easy to test and maintain

### State Machine Pattern
- Encapsulates state transition logic
- Enforces valid state transitions
- Provides hooks for state entry/exit actions
- Thread-safe operations

### CQRS with MediatR
- Commands for state changes
- Queries for data retrieval
- Clean separation of read/write operations

### Repository Pattern
- Entity Framework Core with DbContext
- In-memory database for demonstration
- Easy to switch to other database providers

## 🧪 Testing the State Machine

You can test various state transitions through the Swagger UI:

1. **Create a new order** - Starts in `Pending` state
2. **Transition to `ProcessingPayment`** - Simulates payment processing
3. **Observe automatic transitions** - Based on payment success/failure
4. **View state history** - See complete transition timeline
5. **Try invalid transitions** - System prevents invalid state changes

## 🔧 Configuration

### Database
- Currently uses Entity Framework In-Memory database
- Can be easily switched to SQL Server, PostgreSQL, etc.
- See `Program.cs` for database configuration

### Logging
- Configured for Development environment
- State transitions are logged for debugging
- Configurable log levels in `appsettings.json`

## 📈 Extending the System

### Adding New States
1. Add state to `OrderState` enum
2. Configure transitions in `OrderStateMachineFactory`
3. Add business logic for state entry/exit actions

### Adding New Features
1. Create new folder under `Features/`
2. Implement Command/Query handlers
3. Add controller endpoints if needed
4. Follow existing vertical slice patterns

### Custom Business Rules
State machines can be customized with:
- Entry/exit actions
- Transition guards
- Custom validation logic
- Event handlers

## 🤝 Contributing

1. Follow the existing vertical slice architecture
2. Add appropriate tests for new features
3. Update API documentation
4. Follow C# coding conventions

## 📄 License

This project is for demonstration purposes. Feel free to use and modify as needed.

---

*This project demonstrates modern .NET development practices including Vertical Slice Architecture, State Machine patterns, and clean API design.*
