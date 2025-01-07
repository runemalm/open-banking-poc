using DDD.Domain.Model;

namespace Demo.Domain.Model.Integration
{
	public interface IIntegrationRepository : IRepository<Integration, Guid>
	{
		Task<Integration> GetByNameAsync(string name);
	}
}
