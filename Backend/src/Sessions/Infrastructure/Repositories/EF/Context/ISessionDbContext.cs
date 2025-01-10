using Sessions.Domain.Model;
using Sessions.Domain.Model.Bank;
using Sessions.Domain.Model.Input;
using Sessions.Domain.Model.Integration;
using Microsoft.EntityFrameworkCore;

namespace Sessions.Infrastructure.Repositories.EF.Context
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
