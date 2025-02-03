using Microsoft.EntityFrameworkCore;
using OpenDDD.API.Options;
using OpenDDD.Infrastructure.Persistence.EfCore.Base;

namespace Sessions.Infrastructure.Persistence.EfCore.Context
{
    public class SessionDbContext : OpenDddDbContextBase
    {
        public SessionDbContext(DbContextOptions<SessionDbContext> options, OpenDddOptions openDddOptions, ILogger<SessionDbContext> logger)
            : base(options, openDddOptions, logger)
        {
            
        }
    }
}
