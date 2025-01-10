using DDD.Infrastructure.Repositories.EF;
using Sessions.Domain.Model.Input;
using Sessions.Infrastructure.Repositories.EF.Context;

namespace Sessions.Infrastructure.Repositories.EF
{
    public class EfInputRepository : EfRepositoryBase<Input, Guid, SessionDbContext>, IInputRepository
    {
        public EfInputRepository(ISessionDbContext context) : base((SessionDbContext)context)
        {
            
        }
    }
}
