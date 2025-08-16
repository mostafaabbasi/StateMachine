using MediatR;
using StateMachine.Domain;

namespace StateMachine.Features.Orders.GetOrderStates;

internal sealed class GetOrderStatesHandler : IRequestHandler<GetOrderStatesQuery, GetOrderStatesResult>
{
    public Task<GetOrderStatesResult> Handle(GetOrderStatesQuery request, CancellationToken cancellationToken)
    {
        var states = Enum.GetNames<OrderState>();
        return Task.FromResult(new GetOrderStatesResult(states));
    }
}