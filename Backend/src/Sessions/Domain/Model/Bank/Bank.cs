using OpenDDD.Domain.Model;
using OpenDDD.Domain.Model.Base;

namespace Sessions.Domain.Model.Bank
{
    public class Bank : AggregateRootBase<Guid>
    {
        public string Name { get; private set; }

        // Navigation property
        private readonly List<Integration.Integration> _integrations = new();
        public IReadOnlyCollection<Integration.Integration> Integrations => _integrations.AsReadOnly();

        private Bank(Guid id, string name) : base(id)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public static Bank Create(string name)
        {
            return new Bank(Guid.NewGuid(), name);
        }

        public void AddIntegration(Integration.Integration integration)
        {
            _integrations.Add(integration);
        }
    }
}
