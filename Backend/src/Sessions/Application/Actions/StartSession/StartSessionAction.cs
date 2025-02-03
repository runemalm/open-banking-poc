using OpenDDD.Application;
using Sessions.Domain.Model;
using Sessions.Domain.Services;

namespace Sessions.Application.Actions.StartSession
{
    public class StartSessionAction : IAction<StartSessionCommand, Session>
    {
        private readonly ISessionDomainService _sessionDomainService;
        private readonly ISessionRepository _sessionRepository;

        public StartSessionAction(ISessionDomainService sessionDomainService, ISessionRepository sessionRepository)
        {
            _sessionDomainService = sessionDomainService;
            _sessionRepository = sessionRepository;
        }

        public async Task<Session> ExecuteAsync(StartSessionCommand command, CancellationToken ct)
        {
            await _sessionRepository.GetAsync(command.SessionId, ct);
            var session = await _sessionDomainService.StartSessionAsync(command.SessionId, ct);
            return session;
        }
    }
}
