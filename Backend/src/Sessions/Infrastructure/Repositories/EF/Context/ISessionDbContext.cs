using Demo.Domain.Model;
using Demo.Domain.Model.Bank;
using Demo.Domain.Model.Input;
using Demo.Domain.Model.Integration;
using Microsoft.EntityFrameworkCore;

namespace Demo.Infrastructure.Repositories.EF.Context
{
    public interface ISessionDbContext
    {
        DbSet<Bank> Banks { get; set; }
        DbSet<Integration> Integrations { get; set; }
        DbSet<Session> Sessions { get; set; }
        DbSet<Input> Inputs { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
