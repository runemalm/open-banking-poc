using Microsoft.EntityFrameworkCore;
using DDD.Infrastructure.Repositories.EF.Context;
using Demo.Domain.Model;
using Demo.Domain.Model.Bank;
using Demo.Domain.Model.Input;
using Demo.Domain.Model.Integration;

namespace Demo.Infrastructure.Repositories.EF.Context
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
