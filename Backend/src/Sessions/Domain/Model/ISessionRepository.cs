using DDD.Domain.Model;

namespace Demo.Domain.Model
{
	public interface ISessionRepository : IRepository<Session, Guid>
	{
		
	}
}
