using DDD.Application;
using Sessions.Domain.Model;
using Sessions.Domain.Services;

namespace Sessions.Application.Actions.GetState
{
    public class GetStateAction : IAction<GetStateCommand, (State, Domain.Model.Input.Input)>
    {
        private readonly ISessionDomainService _sessionDomainService;

        public GetStateAction(ISessionDomainService sessionDomainService)
        {
            _sessionDomainService = sessionDomainService;
        }

        public async Task<(State, Domain.Model.Input.Input)> ExecuteAsync(GetStateCommand command, CancellationToken ct)
        {
            var (currentState, input) = await _sessionDomainService.GetCurrentStateAndInputAsync(command.SessionId, ct);
            return (currentState, input);
        }
    }
}
