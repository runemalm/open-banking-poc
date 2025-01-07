using DDD.Infrastructure.Repositories.EF;
using Demo.Domain.Model.Integration;
using Demo.Infrastructure.Repositories.EF.Context;

namespace Demo.Infrastructure.Repositories.EF
{
    public class EfIntegrationRepository : EfRepositoryBase<Integration, Guid, SessionDbContext>, IIntegrationRepository
    {
        public EfIntegrationRepository(ISessionDbContext context) : base((SessionDbContext)context)
        {
            
        }

        public async Task<Integration> GetByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));

            var integrations = await FindWithAsync(i => i.Name == name);
            var integration = integrations.FirstOrDefault();

            if (integration == null)
                throw new InvalidOperationException($"Integration with name '{name}' does not exist.");

            return integration;
        }
    }
}
