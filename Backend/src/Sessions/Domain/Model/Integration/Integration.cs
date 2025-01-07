using DDD.Domain.Model;

namespace Demo.Domain.Model.Integration
{
    public class Integration : AggregateRootBase<Guid>
    {
        public string Name { get; private set; }
        public string ClientDisplayName { get; private set; } = null!;
        
        // Navigation property
        public Guid BankId { get; private set; }
        public Bank.Bank Bank { get; private set; } = null!;

        private Integration(Guid id, string name, string clientDisplayName, Guid bankId) : base(id)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ClientDisplayName = clientDisplayName ?? throw new ArgumentNullException(nameof(clientDisplayName));
            BankId = bankId;
        }

        public static Integration Create(string name, string clientDisplayName, Guid bankId)
        {
            return new Integration(Guid.NewGuid(), name, clientDisplayName, bankId);
        }
    }
}
