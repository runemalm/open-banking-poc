using Demo.Domain.Model.Input;
using Stateless;

namespace Demo.Domain.Model.StateMachine
{
    public abstract class StateMachineBase
    {
        private StateMachine<State, Signal> _stateMachine = null!;
        private readonly ISessionRepository _sessionRepository;
        private readonly IInputRepository _inputRepository;
        public Session Session = null!;
        public event Action<State, State>? OnStateChanged;
        protected readonly IConfiguration _configuration;

        public StateMachineBase(ISessionRepository sessionRepository, IInputRepository inputRepository, IConfiguration configuration)
        {
            _sessionRepository = sessionRepository;
            _inputRepository = inputRepository;
            _configuration = configuration;
        }

        public void InitializeForSession(Session session)
        {
            Session = session;
            
            _stateMachine = new StateMachine<State, Signal>(session.State);
            _stateMachine.OnTransitioned(transition =>
            {
                OnStateChanged?.Invoke(transition.Source, transition.Destination);
            });
            
            ConfigureStates();
        }
        
        private void ConfigureStates()
        {
            _stateMachine.Configure(State.Created)
                .Permit(Signal.Created, State.SelectBank);

            _stateMachine.Configure(State.SelectBank)
                .Permit(Signal.BankSelected, State.SelectIntegration);

            _stateMachine.Configure(State.SelectIntegration)
                .Permit(Signal.IntegrationSelected, State.ReadyToStart);
            
            _stateMachine.Configure(State.ReadyToStart)
                .Permit(Signal.Started, State.Authenticate)
                .Permit(Signal.Failed, State.Failed);

            _stateMachine.Configure(State.Authenticate)
                .OnEntryAsync(async () => await DoAuthenticateAsync())
                .PermitIf(Signal.Authenticated, State.Completed, () => Session.Type == SessionType.Authenticate)
                .PermitIf(Signal.Authenticated, State.FetchingBankAccounts, () => Session.Type == SessionType.GetBankAccounts)
                .PermitIf(Signal.Authenticated, State.FetchingBankAccounts, () => Session.Type == SessionType.GetTransactionHistory)
                .Permit(Signal.Failed, State.Failed);

            _stateMachine.Configure(State.FetchingBankAccounts)
                .OnEntryAsync(async () => await DoFetchingBankAccountsAsync())
                .PermitIf(Signal.BankAccountsFetched, State.Completed, () => Session.Type == SessionType.GetBankAccounts)
                .PermitIf(Signal.BankAccountsFetched, State.SelectBankAccount, () => Session.Type == SessionType.GetTransactionHistory)
                .Permit(Signal.Failed, State.Failed);
            
            _stateMachine.Configure(State.SelectBankAccount)
                .OnEntryAsync(async () => await DoSelectBankAccountAsync())
                .PermitIf(Signal.BankAccountSelected, State.FetchingTransactionHistory, () => Session.Type == SessionType.GetTransactionHistory)
                .Permit(Signal.Failed, State.Failed);

            _stateMachine.Configure(State.FetchingTransactionHistory)
                .OnEntryAsync(async () => await DoFetchingTransactionHistoryAsync())
                .Permit(Signal.TransactionHistoryFetched, State.Completed)
                .Permit(Signal.Failed, State.Failed);

            _stateMachine.Configure(State.Completed)
                .OnEntryAsync(async () => await DoCompletedAsync());
            
            _stateMachine.Configure(State.Failed)
                .OnEntryAsync(async () => await DoFailedAsync());
        }
        
        // Implement by subclasses
        
        protected abstract Task DoAuthenticateAsync();
        protected abstract Task DoFetchingBankAccountsAsync();
        protected abstract Task DoSelectBankAccountAsync();
        protected abstract Task DoFetchingTransactionHistoryAsync();
        protected abstract Task CleanupAsync();
        
        // Common state actions

        private async Task DoCompletedAsync()
        {
            Console.WriteLine("Calling cleanup on subclass in state machine base (completed session state)..");
            await CleanupAsync();
        }

        private async Task DoFailedAsync()
        {
            Console.WriteLine("Calling cleanup on subclass in state machine base (failed session state)..");
            await CleanupAsync();
        }
        
        // Signal

        public async Task FireAsync(Signal signal, object? parameter = null)
        {
            if (parameter != null)
            {
                await _stateMachine.FireAsync(signal, parameter);
            }
            else
            {
                await _stateMachine.FireAsync(signal);
            }
        }

        // Helpers
        
        protected async Task RequestInputAsync(InputRequestType requestType, Dictionary<string, string> data)
        {
            Session.RaiseInputRequested(requestType, data);
            await Task.CompletedTask;
        }
        
        protected async Task<Input.Input> WaitForInputAsync()
        {
            const int PollingIntervalMs = 10000;
            const int MaxWaitTimeMs = 30000;

            var timeoutAt = DateTime.UtcNow.AddMilliseconds(MaxWaitTimeMs);

            while (DateTime.UtcNow < timeoutAt)
            {
                var input = await _inputRepository.GetAsync(Session.Id);

                if (input != null && input.Status == InputStatus.Provided && !string.IsNullOrEmpty(input.Value))
                {
                    Console.WriteLine("Input provided.");
                    return input;
                }

                await Task.Delay(PollingIntervalMs);
            }

            throw new TimeoutException("Timed out waiting for the input to be provided.");
        }
        
        protected async Task SetInputRequestParamAsync(string name, string value)
        {
            var input = await _inputRepository.GetAsync(Session.Id);
            input.SetRequestParam(name, value);
            await _inputRepository.SaveAsync(input, CancellationToken.None);
        }
        
        protected async Task MarkInputProvided()
        {
            var input = await _inputRepository.GetAsync(Session.Id);
            input.Provide("");
            await _inputRepository.SaveAsync(input, CancellationToken.None);
        }
        
        protected async Task MarkInputProvidedAsync(string? value = null)
        {
            var input = await _inputRepository.GetAsync(Session.Id);
            input.Provide(value ?? "");
            await _inputRepository.SaveAsync(input, CancellationToken.None);
        }

        protected async Task RefreshSessionAsync()
        {
            try
            {
                Session = await _sessionRepository.GetAsync(Session.Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error refreshing session: {ex.Message}");
                throw;
            }
        }
    }
}
