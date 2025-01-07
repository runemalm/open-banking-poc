﻿using DDD.Infrastructure.Repositories.EF;
using Demo.Domain.Model.Bank;
using Demo.Infrastructure.Repositories.EF.Context;

namespace Demo.Infrastructure.Repositories.EF
{
    public class EfBankRepository : EfRepositoryBase<Bank, Guid, SessionDbContext>, IBankRepository
    {
        public EfBankRepository(ISessionDbContext context) : base((SessionDbContext)context)
        {
            
        }

        public async Task<Bank> GetByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));

            var banks = await FindWithAsync(b => b.Name == name);
            var bank = banks.FirstOrDefault();

            if (bank == null)
                throw new InvalidOperationException($"Bank with name '{name}' does not exist.");

            return bank;
        }
    }
}
