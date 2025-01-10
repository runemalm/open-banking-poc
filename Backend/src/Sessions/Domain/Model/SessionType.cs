namespace Sessions.Domain.Model
{
    public enum SessionType
    {
        Authenticate,
        GetBankAccounts,
        GetTransactionHistory,
        InitiatePayment
    }
}
