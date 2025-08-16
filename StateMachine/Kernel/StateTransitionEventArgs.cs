namespace StateMachine.Kernel;

public class StateTransitionEventArgs<TState, TContext>(TState toState, TContext context) : EventArgs where TState : struct, Enum
{
    public TState? FromState { get; init; }
    public TState ToState { get; init; } = toState;
    public TContext Context { get; init; } = context ?? throw new ArgumentNullException(nameof(context));
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    public TimeSpan Duration { get; init; }
}