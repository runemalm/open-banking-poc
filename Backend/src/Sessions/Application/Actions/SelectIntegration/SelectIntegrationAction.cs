using OpenDDD.Application;
using OpenDDD.Domain.Model;
using Hangfire;
using Sessions.Application.Actions.StartSession;
using Sessions.Domain.Model;
using Sessions.Domain.Services;

namespace Sessions.Application.Actions.SelectIntegration
{
    public class SelectIntegrationAction : IAction<SelectIntegrationCommand, Session>
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly ISessionDomainService _sessionDomainService;
        private readonly IDomainPublisher _domainPublisher;

        public SelectIntegrationAction(
            ISessionRepository sessionRepository, 
            ISessionDomainService sessionDomainService,
            IDomainPublisher domainPublisher)
        {
            _sessionRepository = sessionRepository;
            _sessionDomainService = sessionDomainService;
            _domainPublisher = domainPublisher;
        }

        public async Task<Session> ExecuteAsync(SelectIntegrationCommand command, CancellationToken ct)
        {
            var session = await _sessionRepository.GetAsync(command.SessionId, ct)
                          ?? throw new InvalidOperationException($"Session {command.SessionId} not found.");
            
            await _sessionDomainService.SelectIntegrationAsync(session, command.IntegrationId, ct);
            
            await _sessionRepository.SaveAsync(session, ct);

            if (session.State == State.ReadyToStart)
            {
                // await _domainPublisher.PublishAsync(new SessionReadyToStart(session.Id), ct);
                
                BackgroundJob.Enqueue<StartSessionAction>(action =>
                    action.ExecuteAsync(new StartSessionCommand(session.Id), CancellationToken.None));
            }

            return session;
        }
    }
}
