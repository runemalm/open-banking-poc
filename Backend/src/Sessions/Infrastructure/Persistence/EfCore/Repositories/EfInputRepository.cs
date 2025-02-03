using OpenDDD.Infrastructure.Persistence.UoW;
using OpenDDD.Infrastructure.Repository.EfCore;
using Sessions.Domain.Model.Input;

namespace Sessions.Infrastructure.Persistence.EfCore.Repositories
{
    public class EfInputRepository : EfCoreRepository<Input, Guid>, IInputRepository
    {
        public EfInputRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
