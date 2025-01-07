using DDD.Application;
using Demo.Domain.Model;
using Demo.Domain.Services;

namespace Demo.Application.Actions.SelectBank
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
            var session = await _sessionRepository.GetAsync(command.SessionId)
                          ?? throw new InvalidOperationException($"Session {command.SessionId} not found.");
            
            await _sessionDomainService.SelectBankAsync(session, command.BankId, ct);
            await _sessionRepository.SaveAsync(session, ct);
            
            return session;
        }
    }
}
