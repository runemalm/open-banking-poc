using DDD.Domain.Model;

namespace Demo.Domain.Model.Bank
{
	public interface IBankRepository : IRepository<Bank, Guid>
	{
		Task<Bank> GetByNameAsync(string name);
	}
}
