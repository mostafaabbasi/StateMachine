using System.ComponentModel.DataAnnotations;
using StateMachine.Domain;

namespace StateMachine.API.DTOs;

public sealed record TransitionOrderRequest(
    [Required] OrderState TargetState,
    string? Notes = null);