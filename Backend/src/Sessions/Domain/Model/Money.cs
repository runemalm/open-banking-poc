namespace Sessions.Domain.Model
{
    public class Money
    {
        public decimal Amount { get; set; }
        public CurrencyType Currency { get; set; }

        public Money(decimal amount, CurrencyType currency)
        {
            Amount = amount;
            Currency = currency;
        }
    }
}
