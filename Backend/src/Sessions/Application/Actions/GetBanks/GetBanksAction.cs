using DDD.Application;
using Demo.Domain.Model.Bank;

namespace Demo.Application.Actions.GetBanks
{
    public class GetBanksAction : IAction<GetBanksCommand, List<Bank>>
    {
        private readonly IBankRepository _bankRepository;

        public GetBanksAction(IBankRepository bankRepository)
        {
            _bankRepository = bankRepository;
        }

        public async Task<List<Bank>> ExecuteAsync(GetBanksCommand command, CancellationToken ct)
        {
            var banks = await _bankRepository.FindAllAsync();
            return banks.ToList();
        }
    }
}
