namespace StateMachine.Kernel;

public class StateConfiguration<TState, TContext>(TState state) where TState : struct, Enum
{
    public TState State { get; } = state;
    public Func<TContext, CancellationToken, Task>? OnEnter { get; set; }
    public Func<TContext, CancellationToken, Task>? OnExit { get; set; }
    public Func<TState, TContext, CancellationToken, Task<bool>>? CanTransitionTo { get; set; }
    public string? DisplayName { get; set; } = state.ToString();

    public StateConfiguration<TState, TContext> OnEnterAsync(Func<TContext, CancellationToken, Task> onEnter)
    { OnEnter = onEnter; return this; }

    public StateConfiguration<TState, TContext> OnEnterAsync(Func<TContext, Task> onEnter)
    { OnEnter = (context, _) => onEnter(context); return this; }

    public StateConfiguration<TState, TContext> OnEnterSync(Action<TContext> onEnter)
    { OnEnter = (context, _) => { onEnter(context); return Task.CompletedTask; }; return this; }

    public StateConfiguration<TState, TContext> OnExitAsync(Func<TContext, CancellationToken, Task> onExit)
    { OnExit = onExit; return this; }

    public StateConfiguration<TState, TContext> WithDisplayName(string displayName)
    { DisplayName = displayName; return this; }

    public StateConfiguration<TState, TContext> CanTransitionToWhen(Func<TState, TContext, bool> canTransitionTo)
    { CanTransitionTo = (targetState, context, _) => Task.FromResult(canTransitionTo(targetState, context)); return this; }
}