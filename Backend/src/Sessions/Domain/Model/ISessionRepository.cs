using DDD.Domain.Model;

namespace Sessions.Domain.Model
{
	public interface ISessionRepository : IRepository<Session, Guid>
	{
		
	}
}
