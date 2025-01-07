using DDD.Domain.Model;
using Demo.Domain.Model.Input;
using Demo.Domain.Model.StateMachine;

namespace Demo.Domain.Model
{
    public class Session : AggregateRootBase<Guid>
    {
        public State State;
        public SessionType Type { get; private set; }
        public bool IsStarted { get; set; }
        
        public string? SelectedBankId { get; set; }
        public string? SelectedIntegrationId { get; set; }
        public string? SelectedBankAccountId { get; set; }
        
        // Related entities
        public User.User? User { get; private set; }
        public BankAccounts.BankAccounts? BankAccounts { get; private set; }
        public TransactionHistory? TransactionHistory { get; private set; }

        // Events

        public event Action<State, State>? OnStateChanged;
        public event Action<InputRequestType, Dictionary<string, string>>? OnInputRequested;
        internal void RaiseInputRequested(InputRequestType requestType, Dictionary<string, string> data)
        {
            OnInputRequested?.Invoke(requestType, data);
        }
        
        private Session(Guid id) : base(id) { }

        public static async Task<Session> CreateAsync(SessionType type, string? bankId, string? integrationId)
        {
            if (integrationId != null && bankId == null)
            {
                throw new InvalidOperationException("Session can't be created with integration but without bank.");
            }
            
            var session = new Session(Guid.NewGuid())
            {
                Type = type,
                IsStarted = false,
                SelectedBankId = bankId,
                SelectedIntegrationId = integrationId
            };

            // No state is bound at this stage, so drive transitions "manually" here
            if (!string.IsNullOrEmpty(integrationId)) session.State = State.ReadyToStart;
            else if (!string.IsNullOrEmpty(bankId)) session.State = State.SelectIntegration;
            else session.State = State.SelectBank;

            return session;
        }

        public async Task SelectBankAsync(string? bankId)
        {
            if (_stateMachine != null)
                throw new Exception("Bank or integration can't be changed once state machine has been set.");
            
            // No state is bound at this stage, so drive transitions "manually" here
            if (string.IsNullOrEmpty(bankId))
            {
                if (State != State.SelectIntegration)
                {
                    throw new Exception("You must be in state 'SelectIntegration' if you want to unset the selected bank");
                }
                
                SelectedBankId = null;
                State = State.SelectBank;
            }
            else
            {
                EnsureState(State.SelectBank);
                SelectedBankId = bankId;
                State = State.SelectIntegration;
            }
        }
        
        public async Task SelectIntegrationAsync(string integrationId)
        {
            if (_stateMachine != null)
                throw new Exception("Bank or integration can't be changed once state machine has been set.");
            
            // No state is bound at this stage, so drive transitions "manually" here
            EnsureState(State.SelectIntegration);
            SelectedIntegrationId = integrationId;
            State = State.ReadyToStart;

            if (string.IsNullOrEmpty(SelectedIntegrationId))
            {
                SelectedIntegrationId = null;
                SelectedBankId = null;
                State = State.SelectBank;
            }
        }
        
        public async Task StartAsync()
        {
            EnsureState(State.ReadyToStart);
            if (_stateMachine == null) throw new Exception("State machine must be set before start.");
            IsStarted = true;
            await _stateMachine!.FireAsync(Signal.Started);
        }
        
        public async Task SelectBankAccountAsync(string bankAccountId)
        {
            throw new NotImplementedException();
        }
        
        // Related entities

        public void SetUser(User.User user)
        {
            User = user ?? throw new ArgumentNullException(nameof(user));
        }

        public void SetBankAccounts(BankAccounts.BankAccounts bankAccounts)
        {
            BankAccounts = bankAccounts ?? throw new ArgumentNullException(nameof(bankAccounts));
        }

        public void SetTransactionHistory(TransactionHistory history)
        {
            TransactionHistory = history ?? throw new ArgumentNullException(nameof(history));
        }
        
        // State machine
        
        private StateMachineBase? _stateMachine;
        
        public void BindStateMachine(StateMachineBase stateMachine)
        {
            _stateMachine = stateMachine;

            stateMachine.OnStateChanged += (oldState, newState) =>
            {
                State = newState;
                OnStateChanged?.Invoke(oldState, newState);
            };
        }
        
        private void EnsureState(State expectedState)
        {
            if (State != expectedState)
            {
                throw new InvalidOperationException(
                    $"Invalid state transition. Expected state: {expectedState}, Actual state: {State}, Session ID: {Id}.");
            }
        }
    }
}
