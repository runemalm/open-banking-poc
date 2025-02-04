using OpenDDD.Application;
using OpenDDD.Domain.Model;
using Hangfire;
using Sessions.Application.Actions.StartSession;
using Sessions.Domain.Model;
using Sessions.Domain.Model.Input;
using Sessions.Domain.Services;

namespace Sessions.Application.Actions.CreateSession
{
    public class CreateSessionAction : IAction<CreateSessionCommand, (Session, Input)>
    {
        private readonly ISessionDomainService _sessionDomainService;
        private readonly ISessionRepository _sessionRepository;
        private readonly IInputRepository _inputRepository;
        private readonly IDomainPublisher _domainPublisher;

        public CreateSessionAction(
            ISessionDomainService sessionDomainService, 
            ISessionRepository sessionRepository,
            IInputRepository inputRepository,
            IDomainPublisher domainPublisher)
        {
            _sessionDomainService = sessionDomainService;
            _sessionRepository = sessionRepository;
            _inputRepository = inputRepository;
            _domainPublisher = domainPublisher;
        }

        public async Task<(Session, Input)> ExecuteAsync(CreateSessionCommand command, CancellationToken ct)
        {
            var (session, input) = await _sessionDomainService.CreateSessionAsync(
                command.Type, command.BankId, command.IntegrationId, ct);

            await _sessionRepository.SaveAsync(session, ct);
            await _inputRepository.SaveAsync(input, ct);
            
            if (session.State == State.ReadyToStart)
            {
                // await _domainPublisher.PublishAsync(new SessionReadyToStart(session.Id), ct);
                
                BackgroundJob.Enqueue<StartSessionAction>(action =>
                    action.ExecuteAsync(new StartSessionCommand(session.Id), CancellationToken.None));
            }

            return (session, input);
        }
    }
}
