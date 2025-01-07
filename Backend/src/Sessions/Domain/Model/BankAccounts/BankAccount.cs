using DDD.Domain.Model;

namespace Demo.Domain.Model.BankAccounts
{
    public class BankAccount : IValueObject
    {
        public string Number { get; set; }
        
        private BankAccount() { }
        
        public static BankAccount Create(string number)
        {
            return new BankAccount()
            {
                Number = number ?? throw new ArgumentNullException(nameof(number)),
            };
        }
    }
}
