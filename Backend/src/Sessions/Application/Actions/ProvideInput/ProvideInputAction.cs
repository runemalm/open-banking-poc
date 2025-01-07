using DDD.Application;
using Demo.Domain.Model;
using Demo.Domain.Model.Input;
using Demo.Domain.Services;

namespace Demo.Application.Actions.ProvideInput
{
    public class ProvideInputAction : IAction<ProvideInputCommand, bool>
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly ISessionDomainService _sessionDomainService;
        private readonly IInputRepository _inputRepository;

        public ProvideInputAction(
            ISessionDomainService sessionDomainService, 
            ISessionRepository sessionRepository,
            IInputRepository inputRepository)
        {
            _sessionRepository = sessionRepository;
            _sessionDomainService = sessionDomainService;
            _inputRepository = inputRepository;
        }

        public async Task<bool> ExecuteAsync(ProvideInputCommand command, CancellationToken ct)
        {
            var session = await _sessionRepository.GetAsync(command.SessionId);
            var input = await _sessionDomainService.ProvideInputAsync(session, command.Value, ct);
            await _inputRepository.SaveAsync(input, ct);
            return true;
        }
    }
}
