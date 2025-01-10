namespace Sessions.Domain.Model
{
    public enum Signal
    {
        Created,
        BankSelected,
        IntegrationSelected,
        Started,
        InputProvided,
        Authenticated,
        ConsentProvided,
        BankAccountsFetched,
        BankAccountSelected,
        TransactionHistoryFetched,
        PaymentInitiated,
        Completed,
        Failed
    }
}
