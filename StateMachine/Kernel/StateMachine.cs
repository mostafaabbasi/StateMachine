using System.Collections.Concurrent;
using System.Diagnostics;
using StateMachine.Kernel;

namespace StateMachine.Kernel;

public class StateMachine<TState, TContext> : IStateMachine<TState, TContext>, IDisposable 
    where TState : struct, Enum
{
    private readonly ConcurrentDictionary<TState, StateConfiguration<TState, TContext>> _stateConfigurations = new();
    private readonly ConcurrentDictionary<TState, HashSet<TState>> _transitions = new();
    private readonly SemaphoreSlim _transitionLock = new(1, 1);
    private readonly ILogger<StateMachine<TState, TContext>>? _logger;
    private readonly TContext _context;
    
    private TState _currentState;
    private bool _disposed;

    public TState CurrentState => _currentState;
    public event EventHandler<StateTransitionEventArgs<TState, TContext>>? StateChanged;

    public StateMachine(TState initialState, TContext context, ILogger<StateMachine<TState, TContext>>? logger = null)
    {
        _currentState = initialState;
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger;

        foreach (TState state in Enum.GetValues<TState>())
        {
            _stateConfigurations.TryAdd(state, new StateConfiguration<TState, TContext>(state));
        }
    }

    public IStateMachine<TState, TContext> ConfigureState(TState state, Func<StateConfiguration<TState, TContext>, StateConfiguration<TState, TContext>>? configure = null)
    {
        var config = _stateConfigurations.GetOrAdd(state, new StateConfiguration<TState, TContext>(state));
        configure?.Invoke(config);
        return this;
    }

    public IStateMachine<TState, TContext> AllowTransition(TState fromState, TState toState)
    {
        _transitions.AddOrUpdate(fromState, [toState], (_, existing) => { existing.Add(toState); return existing; });
        return this;
    }

    public IStateMachine<TState, TContext> AllowTransitions(TState fromState, params TState[] toStates)
    {
        foreach (var toState in toStates) AllowTransition(fromState, toState);
        return this;
    }

    public IEnumerable<TState> GetAllowedTransitions(TState fromState)
    {
        return _transitions.TryGetValue(fromState, out var transitions) ? transitions : [];
    }

    public async Task<bool> CanTransitionToAsync(TState targetState, CancellationToken cancellationToken = default)
    {
        if (_currentState.Equals(targetState)) return true;

        if (!_transitions.TryGetValue(_currentState, out var allowedTransitions) || !allowedTransitions.Contains(targetState))
            return false;

        if (_stateConfigurations.TryGetValue(_currentState, out var currentConfig) && currentConfig.CanTransitionTo != null)
        {
            try { return await currentConfig.CanTransitionTo(targetState, _context, cancellationToken); }
            catch (Exception ex) { _logger?.LogError(ex, "Error in transition validation"); return false; }
        }

        return true;
    }

    public async Task<StateTransitionResult<TState>> TransitionToAsync(TState targetState, CancellationToken cancellationToken = default)
    {
        if (_currentState.Equals(targetState))
            return StateTransitionResult<TState>.Succeeded(_currentState, _currentState, TimeSpan.Zero);

        await _transitionLock.WaitAsync(cancellationToken);
        
        try
        {
            var stopwatch = Stopwatch.StartNew();
            var previousState = _currentState;

            if (!await CanTransitionToAsync(targetState, cancellationToken))
            {
                return StateTransitionResult<TState>.Failed(_currentState, 
                    $"Transition from '{_currentState}' to '{targetState}' not allowed");
            }

            try
            {
                if (_stateConfigurations.TryGetValue(_currentState, out var currentConfig) && currentConfig.OnExit != null)
                    await currentConfig.OnExit(_context, cancellationToken);

                _currentState = targetState;
                
                if (_stateConfigurations.TryGetValue(targetState, out var targetConfig) && targetConfig.OnEnter != null)
                    await targetConfig.OnEnter(_context, cancellationToken);

                stopwatch.Stop();

                StateChanged?.Invoke(this, new StateTransitionEventArgs<TState, TContext>(targetState, _context) 
                { FromState = previousState, Duration = stopwatch.Elapsed });

                return StateTransitionResult<TState>.Succeeded(previousState, targetState, stopwatch.Elapsed);
            }
            catch (Exception ex)
            {
                _currentState = previousState;
                return StateTransitionResult<TState>.Failed(previousState, $"Transition failed: {ex.Message}");
            }
        }
        finally
        {
            _transitionLock.Release();
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _transitionLock.Dispose();
        _disposed = true;
        GC.SuppressFinalize(this);
    }
}