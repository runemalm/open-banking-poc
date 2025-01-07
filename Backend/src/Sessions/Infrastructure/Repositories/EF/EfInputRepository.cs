using DDD.Infrastructure.Repositories.EF;
using Demo.Domain.Model.Input;
using Demo.Infrastructure.Repositories.EF.Context;

namespace Demo.Infrastructure.Repositories.EF
{
    public class EfInputRepository : EfRepositoryBase<Input, Guid, SessionDbContext>, IInputRepository
    {
        public EfInputRepository(ISessionDbContext context) : base((SessionDbContext)context)
        {
            
        }
    }
}
