using OpenDDD.Infrastructure.Persistence.UoW;
using OpenDDD.Infrastructure.Repository.EfCore;
using Sessions.Domain.Model.Integration;

namespace Sessions.Infrastructure.Persistence.EfCore.Repositories
{
    public class EfIntegrationRepository : EfCoreRepository<Integration, Guid>, IIntegrationRepository
    {
        public EfIntegrationRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<Integration> GetByNameAsync(string name, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));

            var integrations = await FindWithAsync(i => i.Name == name, ct);
            var integration = integrations.FirstOrDefault();

            if (integration == null)
                throw new InvalidOperationException($"Integration with name '{name}' does not exist.");

            return integration;
        }
    }
}
