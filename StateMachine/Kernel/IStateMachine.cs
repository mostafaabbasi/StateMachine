namespace StateMachine.Kernel;

public interface IStateMachine<TState, TContext> where TState : struct, Enum
{
    TState CurrentState { get; }
    Task<StateTransitionResult<TState>> TransitionToAsync(TState targetState, CancellationToken cancellationToken = default);
    Task<bool> CanTransitionToAsync(TState targetState, CancellationToken cancellationToken = default);
    IStateMachine<TState, TContext> ConfigureState(TState state, Func<StateConfiguration<TState, TContext>, StateConfiguration<TState, TContext>>? configure = null);
    IStateMachine<TState, TContext> AllowTransition(TState fromState, TState toState);
    IStateMachine<TState, TContext> AllowTransitions(TState fromState, params TState[] toStates);
    IEnumerable<TState> GetAllowedTransitions(TState fromState);
    event EventHandler<StateTransitionEventArgs<TState, TContext>> StateChanged;
}