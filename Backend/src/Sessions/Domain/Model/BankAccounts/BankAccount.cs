using System.ComponentModel.DataAnnotations.Schema;
using OpenDDD.Domain.Model;

namespace Sessions.Domain.Model.BankAccounts
{
    public class BankAccount : IValueObject
    {
        public string Number { get; set; }
        
        public BankAccount() { }
        
        public static BankAccount Create(string number)
        {
            return new BankAccount()
            {
                Number = number ?? throw new ArgumentNullException(nameof(number)),
            };
        }
    }
}
