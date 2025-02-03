using OpenDDD.Infrastructure.Persistence.UoW;
using OpenDDD.Infrastructure.Repository.EfCore;
using Sessions.Domain.Model;

namespace Sessions.Infrastructure.Persistence.EfCore.Repositories
{
    public class EfSessionRepository : EfCoreRepository<Session, Guid>, ISessionRepository
    {
        public EfSessionRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
