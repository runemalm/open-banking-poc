using DDD.Domain.Model;

namespace Demo.Domain.Model.BankAccounts
{
    public class BankAccounts : IValueObject
    {
        public List<BankAccount> Accounts { get; set; }

        private BankAccounts() { }

        public static BankAccounts Create(List<BankAccount> accounts)
        {
            var bankAccounts = new BankAccounts()
            {
                Accounts = accounts
            };
            return bankAccounts;
        }

        public static BankAccounts NoAccounts()
        {
            var bankAccounts = new BankAccounts()
            {
                Accounts = new List<BankAccount>()
            };
            return bankAccounts;
        }
    }
}
