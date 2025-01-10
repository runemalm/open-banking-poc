using Microsoft.EntityFrameworkCore;
using DDD.Infrastructure.Repositories.EF.Context;
using Sessions.Domain.Model;
using Sessions.Domain.Model.Bank;
using Sessions.Domain.Model.Input;
using Sessions.Domain.Model.Integration;

namespace Sessions.Infrastructure.Repositories.EF.Context
{
    public class SessionDbContext : EfDbContextBase, ISessionDbContext
    {
        public SessionDbContext(DbContextOptions<SessionDbContext> options) : base(options) { }

        public DbSet<Bank> Banks { get; set; }
        public DbSet<Integration> Integrations { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Input> Inputs { get; set; }
    }
}
