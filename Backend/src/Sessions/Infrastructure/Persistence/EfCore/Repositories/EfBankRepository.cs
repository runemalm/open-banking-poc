using OpenDDD.Infrastructure.Persistence.UoW;
using OpenDDD.Infrastructure.Repository.EfCore;
using Sessions.Domain.Model.Bank;

namespace Sessions.Infrastructure.Persistence.EfCore.Repositories
{
    public class EfBankRepository : EfCoreRepository<Bank, Guid>, IBankRepository
    {
        private readonly ILogger<EfBankRepository> _logger;

        public EfBankRepository(IUnitOfWork unitOfWork, ILogger<EfBankRepository> logger) 
            : base(unitOfWork)
        {
            _logger = logger;
        }
        
        public async Task<Bank> GetByNameAsync(string name, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));

            var banks = await FindWithAsync(b => b.Name == name, ct);
            var bank = banks.FirstOrDefault();

            if (bank == null)
                throw new InvalidOperationException($"Bank with name '{name}' does not exist.");

            return bank;
        }
    }
}
