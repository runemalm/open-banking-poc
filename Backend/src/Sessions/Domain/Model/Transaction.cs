using DDD.Domain.Model;

namespace Demo.Domain.Model
{
    public class Transaction : IValueObject
    {
        public Money Amount { get; set; } = null;
        public DateTime Date { get; set; }
        public string Description { get; set; } = null;
        public TransactionType Type { get; set; }

        private Transaction() { }

        public static Transaction Create(Money amount, DateTime date, string description, TransactionType type)
        {
            return new Transaction()
            {
                Amount = amount ?? throw new ArgumentNullException(nameof(amount)),
                Date = date,
                Description = description ?? throw new ArgumentNullException(nameof(description)),
                Type = type
            };
        }
    }
}
