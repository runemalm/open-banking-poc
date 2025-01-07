using DDD.Domain.Model;

namespace Demo.Domain.Model
{
    public class TransactionHistory : IValueObject
    {
        public List<Transaction> Transactions { get; set; }

        private TransactionHistory() { }

        public static TransactionHistory Create(List<Transaction> transactions)
        {
            var transactionHistory = new TransactionHistory()
            {
                Transactions = transactions
            };
            return transactionHistory;
        }
        
        public static TransactionHistory NoTransactions()
        {
            var transactionHistory = new TransactionHistory()
            {
                Transactions = new List<Transaction>()
            };
            return transactionHistory;
        }
    }
}
