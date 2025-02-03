using OpenDDD.Application;
using Sessions.Domain.Model;
using Sessions.Domain.Services;

namespace Sessions.Application.Actions.SelectBank
{
    public class SelectBankAction : IAction<SelectBankCommand, Session>
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly ISessionDomainService _sessionDomainService;

        public SelectBankAction(ISessionRepository sessionRepository, ISessionDomainService sessionDomainService)
        {
            _sessionRepository = sessionRepository;
            _sessionDomainService = sessionDomainService;
        }

        public async Task<Session> ExecuteAsync(SelectBankCommand command, CancellationToken ct)
        {
            var session = await _sessionRepository.GetAsync(command.SessionId, ct)
                          ?? throw new InvalidOperationException($"Session {command.SessionId} not found.");
            
            await _sessionDomainService.SelectBankAsync(session, command.BankId, ct);
            await _sessionRepository.SaveAsync(session, ct);
            
            return session;
        }
    }
}
