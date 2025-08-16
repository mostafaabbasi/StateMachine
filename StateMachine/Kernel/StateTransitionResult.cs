namespace StateMachine.Kernel;

public class StateTransitionResult<TState> where TState : struct, Enum
{
public bool Success { get; init; }
public TState? PreviousState { get; init; }
public TState CurrentState { get; init; }
public string? ErrorMessage { get; init; }
public TimeSpan Duration { get; init; }
public DateTime Timestamp { get; init; } = DateTime.UtcNow;

public static StateTransitionResult<TState> Succeeded(TState previousState, TState currentState, TimeSpan duration) =>
    new() { Success = true, PreviousState = previousState, CurrentState = currentState, Duration = duration };

public static StateTransitionResult<TState> Failed(TState currentState, string errorMessage) =>
    new() { Success = false, CurrentState = currentState, ErrorMessage = errorMessage };
}