using DDD.Application;
using Demo.Domain.Model;
using Demo.Domain.Services;

namespace Demo.Application.Actions.StartSession
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
            await _sessionRepository.GetAsync(command.SessionId);
            var session = await _sessionDomainService.StartSessionAsync(command.SessionId, ct);
            return session;
        }
    }
}
