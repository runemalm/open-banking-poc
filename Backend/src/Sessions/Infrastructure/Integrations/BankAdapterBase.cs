using Demo.Domain.Model;
using Demo.Domain.Model.BankAccounts;
using Demo.Domain.Model.Input;
using Demo.Domain.Model.Ports;
using Demo.Domain.Model.User;

namespace Demo.Infrastructure.BankAdapters
{
    public abstract class BankAdapterBase : IBankPort
    {
        public event Func<InputRequestType, Dictionary<string, string?>?, Task>? OnInputRequested;
        
        public abstract Task StartSessionAsync(SessionType sessionType);
        public abstract Task CancelSessionAsync();
        public abstract Task<BankPortResult<User>> AuthenticateAsync();
        public abstract Task<BankPortResult<BankAccounts>> FetchBankAccountsAsync();
        public abstract Task<BankPortResult<TransactionHistory>> FetchTransactionHistoryAsync();
        
        protected async Task RequestInputAsync(InputRequestType requestType, Dictionary<string, string?>? requestParams = null)
        {
            if (OnInputRequested != null)
            {
                await OnInputRequested.Invoke(requestType, requestParams);
            }
            else
            {
                throw new InvalidOperationException("No handler for input requests.");
            }
        }
        
        protected void MarkInputAsError(InputError error, string? errorMessage = null)
        {
            throw new NotImplementedException();
        }
    }
}
