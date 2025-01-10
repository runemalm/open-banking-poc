using Sessions.Domain.Model.Input;

namespace Sessions.Domain.Model.Ports
{
    public interface IBankPort
    {
        event Func<InputRequestType, Dictionary<string, string?>?, Task>? OnInputRequested;

        Task StartSessionAsync(SessionType sessionType);
        Task CancelSessionAsync();
        Task<BankPortResult<User.User>> AuthenticateAsync();
        Task<BankPortResult<BankAccounts.BankAccounts>> FetchBankAccountsAsync();
        Task<BankPortResult<TransactionHistory>> FetchTransactionHistoryAsync();
    }
}
