using OpenDDD.Domain.Model;

namespace Sessions.Domain.Model
{
    public class TransactionHistory : IValueObject
    {
        public List<Transaction> Transactions { get; set; }

        public TransactionHistory() { }

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
