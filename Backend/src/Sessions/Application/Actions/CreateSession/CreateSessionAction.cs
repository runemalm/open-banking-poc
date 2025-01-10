using DDD.Application;
using Sessions.Application.Actions.StartSession;
using Sessions.Domain.Model;
using Sessions.Domain.Model.Input;
using Sessions.Domain.Services;
using Hangfire;
using MediatR;

namespace Sessions.Application.Actions.CreateSession
{
    public class CreateSessionAction : IAction<CreateSessionCommand, (Session, Domain.Model.Input.Input)>
    {
        private readonly ISessionDomainService _sessionDomainService;
        private readonly ISessionRepository _sessionRepository;
        private readonly IInputRepository _inputRepository;
        private readonly IMediator _mediator;

        public CreateSessionAction(
            ISessionDomainService sessionDomainService, 
            ISessionRepository sessionRepository,
            IInputRepository inputRepository,
            IMediator mediator)
        {
            _sessionDomainService = sessionDomainService;
            _sessionRepository = sessionRepository;
            _inputRepository = inputRepository;
            _mediator = mediator;
        }

        public async Task<(Session, Input)> ExecuteAsync(CreateSessionCommand command, CancellationToken ct)
        {
            var (session, input) = await _sessionDomainService.CreateSessionAsync(
                command.Type, command.BankId, command.IntegrationId, ct);

            await _sessionRepository.SaveAsync(session, ct);
            await _inputRepository.SaveAsync(input, ct);
            
            if (session.State == State.ReadyToStart)
            {
                // await _mediator.Publish(new SessionReadyToStart(session.Id), ct);
                
                BackgroundJob.Enqueue<StartSessionAction>(action =>
                    action.ExecuteAsync(new StartSessionCommand(session.Id), CancellationToken.None));
            }

            return (session, input);
        }
    }
}
