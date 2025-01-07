using MediatR;
using DDD.Application;
using Demo.Application.Actions.StartSession;
using Demo.Domain.Model;
using Demo.Domain.Services;
using Hangfire;

namespace Demo.Application.Actions.SelectIntegration
{
    public class SelectIntegrationAction : IAction<SelectIntegrationCommand, Session>
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly ISessionDomainService _sessionDomainService;
        private readonly IMediator _mediator;

        public SelectIntegrationAction(
            ISessionRepository sessionRepository, 
            ISessionDomainService sessionDomainService,
            IMediator mediator)
        {
            _sessionRepository = sessionRepository;
            _sessionDomainService = sessionDomainService;
            _mediator = mediator;
        }

        public async Task<Session> ExecuteAsync(SelectIntegrationCommand command, CancellationToken ct)
        {
            var session = await _sessionRepository.GetAsync(command.SessionId)
                          ?? throw new InvalidOperationException($"Session {command.SessionId} not found.");
            
            await _sessionDomainService.SelectIntegrationAsync(session, command.IntegrationId, ct);
            
            await _sessionRepository.SaveAsync(session, ct);

            if (session.State == State.ReadyToStart)
            {
                // await _mediator.Publish(new SessionReadyToStart(session.Id), ct);
                
                BackgroundJob.Enqueue<StartSessionAction>(action =>
                    action.ExecuteAsync(new StartSessionCommand(session.Id), CancellationToken.None));
            }

            return session;
        }
    }
}
