using Demo.Domain.Model;
using Demo.Domain.Model.Input;
using Demo.Domain.Model.StateMachine.Factory;

namespace Demo.Domain.Services
{
    public class SessionDomainService : ISessionDomainService
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly IInputRepository _inputRepository;
        private readonly IStateMachineFactory _stateMachineFactory;

        public SessionDomainService(
            ISessionRepository sessionRepository,
            IInputRepository inputRepository,
            IStateMachineFactory stateMachineFactory)
        {
            _sessionRepository = sessionRepository;
            _inputRepository = inputRepository;
            _stateMachineFactory = stateMachineFactory;
        }
        
        public async Task<(Session, Input)> CreateSessionAsync(SessionType type, string? bankId, string? integrationId, CancellationToken ct)
        {
            var session = await Session.CreateAsync(type, bankId, integrationId);
            var input = Input.CreateForSession(session.Id);
            return (session, input);
        }
        
        public async Task SelectBankAsync(Session session, string? bankId, CancellationToken ct)
        {
            await session.SelectBankAsync(bankId);
        }

        public async Task SelectIntegrationAsync(Session session, string integrationId, CancellationToken ct)
        {
            await session.SelectIntegrationAsync(integrationId);
        }
        
        public async Task<Session> StartSessionAsync(Guid sessionId, CancellationToken ct)
        {
            var session = await _sessionRepository.GetAsync(sessionId);
            session.BindStateMachine(_stateMachineFactory.CreateForSession(session));
            
            var input = await _inputRepository.GetAsync(sessionId);
            
            session.OnStateChanged += (_, _) =>
            {
                _sessionRepository.SaveAsync(session, CancellationToken.None).GetAwaiter().GetResult();
            };
            
            session.OnInputRequested += (inputRequestType, requestParams) =>
            {
                Console.WriteLine($"Input was requested...");
                input = _inputRepository.GetAsync(sessionId).GetAwaiter().GetResult();
                input.Request(inputRequestType, requestParams);
                _inputRepository.SaveAsync(input, ct).GetAwaiter().GetResult();
            };

            Console.WriteLine($"Starting session {session.Id} of type {session.Type}...");
            
            await session.StartAsync();
            
            await RunSessionUntilEndAsync(session, ct);

            return session;
        }

        private async Task RunSessionUntilEndAsync(Session session, CancellationToken ct)
        {
            while (!new[] { State.Completed, State.Failed }.Contains(session.State))
            {
                ct.ThrowIfCancellationRequested(); // Support for cancellation

                session = await _sessionRepository.GetAsync(session.Id);

                Console.WriteLine($"Session {session.Id} current state: {session.State}");

                await Task.Delay(1000, ct);
            }

            Console.WriteLine($"Session {session.Id} has ended with state: {session.State}");
        }
        
        public async Task<Input> ProvideInputAsync(Session session, string value, CancellationToken ct)
        {
            var input = await _inputRepository.GetAsync(session.Id);
            input.Provide(value);
            return input;
        }
        
        public async Task<(State, Input)> GetCurrentStateAndInputAsync(Guid sessionId, CancellationToken ct)
        {
            var session = await _sessionRepository.GetAsync(sessionId);
            var input = await _inputRepository.GetAsync(sessionId);
            return (session.State, input);
        }
    }
}
