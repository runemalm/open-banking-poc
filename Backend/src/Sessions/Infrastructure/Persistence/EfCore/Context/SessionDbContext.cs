using Microsoft.EntityFrameworkCore;
using OpenDDD.API.Options;
using OpenDDD.Infrastructure.Persistence.EfCore.Base;
using Sessions.Domain.Model;
using Sessions.Domain.Model.Bank;
using Sessions.Domain.Model.Input;
using Sessions.Domain.Model.Integration;

namespace Sessions.Infrastructure.Persistence.EfCore.Context
{
    public class SessionDbContext : OpenDddDbContextBase
    {
        public DbSet<Bank> Banks { get; set; }
        public DbSet<Integration> Integrations { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Input> Inputs { get; set; }

        public SessionDbContext(DbContextOptions<SessionDbContext> options, OpenDddOptions openDddOptions, ILogger<SessionDbContext> logger)
            : base(options, openDddOptions, logger)
        {
            
        }
    }
}
