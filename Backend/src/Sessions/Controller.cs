using Microsoft.AspNetCore.Mvc;
using Demo.Application.Actions.CreateSession;
using Demo.Application.Actions.GetBanks;
using Demo.Application.Actions.GetIntegrations;
using Demo.Application.Actions.GetSession;
using Demo.Application.Actions.GetState;
using Demo.Application.Actions.ProvideInput;
using Demo.Application.Actions.SelectBank;
using Demo.Application.Actions.SelectIntegration;
using Demo.Application.Actions.StartSession;

namespace Demo
{
    [ApiController]
    [Route("api")]
    public class Controller : ControllerBase
    {
        private readonly GetBanksAction _getBanksAction;
        private readonly GetIntegrationsAction _getIntegrationsAction;
        private readonly CreateSessionAction _createSessionAction;
        private readonly SelectBankAction _selectBankAction;
        private readonly SelectIntegrationAction _selectIntegrationAction;
        private readonly StartSessionAction _startSessionAction;
        private readonly ProvideInputAction _provideInputAction;
        private readonly GetStateAction _getStateAction;
        private readonly GetSessionAction _getSessionAction;

        public Controller(CreateSessionAction createSessionAction, GetBanksAction getBanksAction,
            GetIntegrationsAction getIntegrationsAction ,SelectBankAction selectBankAction, 
            SelectIntegrationAction selectIntegrationAction, StartSessionAction startSessionAction,
            ProvideInputAction provideInputAction, GetStateAction getStateAction, GetSessionAction getSessionAction)
        {
            _createSessionAction = createSessionAction;
            _getBanksAction = getBanksAction;
            _getIntegrationsAction = getIntegrationsAction;
            _selectBankAction = selectBankAction;
            _selectIntegrationAction = selectIntegrationAction;
            _startSessionAction = startSessionAction;
            _provideInputAction = provideInputAction;
            _getStateAction = getStateAction;
            _getSessionAction = getSessionAction;
        }

        [HttpGet("get-banks")]
        public async Task<IActionResult> GetBanks(CancellationToken ct)
        {
            var banks = await _getBanksAction.ExecuteAsync(new GetBanksCommand(), ct);
            return Ok(banks.Select(bank => new
            {
                bank.Id, bank.Name
            }));
        }
        
        [HttpGet("get-integrations")]
        public async Task<IActionResult> GetIntegrations([FromQuery] Guid bankId, CancellationToken ct)
        {
            var integrations = await _getIntegrationsAction.ExecuteAsync(new GetIntegrationsCommand(bankId), ct);
            return Ok(integrations.Select(integration => new
            {
                integration.Id,
                integration.Name,
                integration.ClientDisplayName
            }));
        }
        
        [HttpPost("create-session")]
        public async Task<IActionResult> CreateSession([FromBody] CreateSessionCommand command, CancellationToken ct)
        {
            var (session, input) = await _createSessionAction.ExecuteAsync(command, ct);
            var response = SessionResponse.Create(session.Id, session.State, input);
            return Ok(response);
        }
        
        [HttpPost("select-bank")]
        public async Task<IActionResult> SelectBank([FromBody] SelectBankCommand command, CancellationToken ct)
        {
            var session = await _selectBankAction.ExecuteAsync(command, ct);
            var response = SessionResponse.Create(session.Id, session.State, null);
            return Ok(response);
        }

        [HttpPost("select-integration")]
        public async Task<IActionResult> SelectIntegration([FromBody] SelectIntegrationCommand command, CancellationToken ct)
        {
            var session = await _selectIntegrationAction.ExecuteAsync(command, ct);
            var response = SessionResponse.Create(session.Id, session.State, null);
            return Ok(response);
        }
        
        [HttpPost("start-session")]
        public async Task<IActionResult> StartSession([FromBody] StartSessionCommand command, CancellationToken ct)
        {
            var session = await _startSessionAction.ExecuteAsync(command, ct);
            var response = SessionResponse.Create(session.Id, session.State, null);
            return Ok(response);
        }
        
        [HttpPost("provide-input")]
        public async Task<IActionResult> ProvideInput([FromBody] ProvideInputCommand command, CancellationToken ct)
        {
            await _provideInputAction.ExecuteAsync(command, ct);
            return Ok();
        }
        
        [HttpGet("get-state")]
        public async Task<IActionResult> GetState([FromQuery] Guid sessionId, CancellationToken ct)
        {
            var command = new GetStateCommand(sessionId);
            var (currentState, input) = await _getStateAction.ExecuteAsync(command, ct);
            var response = SessionResponse.Create(sessionId, currentState, input);
            return Ok(response);
        }
        
        [HttpGet("get-session")]
        public async Task<IActionResult> GetSession([FromQuery] Guid sessionId, CancellationToken ct)
        {
            var command = new GetSessionCommand(sessionId);
            var session = await _getSessionAction.ExecuteAsync(command, ct);
            return Ok(new
            {
                session.Id,
                session.Type,
                session.State,
                session.SelectedBankId,
                session.SelectedIntegrationId,
                session.CreatedAt,
                session.UpdatedAt,
                session.User?.Name,
                session.User?.Nin
            });
        }
    }
}
