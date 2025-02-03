using OpenDDD.Application;
using Sessions.Domain.Model;
using Sessions.Domain.Model.Input;
using Sessions.Domain.Services;

namespace Sessions.Application.Actions.ProvideInput
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
            var session = await _sessionRepository.GetAsync(command.SessionId, ct);
            var input = await _sessionDomainService.ProvideInputAsync(session, command.Value, ct);
            await _inputRepository.SaveAsync(input, ct);
            return true;
        }
    }
}
