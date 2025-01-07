namespace Demo.Domain.Model
{
    public enum State
    {
        Start,
        Created,
        SelectBank,
        SelectIntegration,
        ReadyToStart,
        Starting,
        Authenticate,
        FetchingBankAccounts,
        SelectBankAccount,
        ProvideConsent,
        FetchingTransactionHistory,
        Completed,
        Failed
    }
}
