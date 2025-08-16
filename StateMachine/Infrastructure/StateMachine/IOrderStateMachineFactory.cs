using StateMachine.Domain;
using StateMachine.Kernel;

namespace StateMachine.Infrastructure.StateMachine;

public interface IOrderStateMachineFactory
{
    IStateMachine<OrderState, Order> Create(Order order);
}