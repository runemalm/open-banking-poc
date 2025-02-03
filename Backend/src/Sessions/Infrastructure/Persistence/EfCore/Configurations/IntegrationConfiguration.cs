using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenDDD.Infrastructure.Persistence.EfCore.Base;
using Sessions.Domain.Model.Integration;

namespace Sessions.Infrastructure.Persistence.EfCore.Configurations
{
    public class IntegrationConfiguration : EfAggregateRootConfigurationBase<Integration, Guid>
    {
        public override void Configure(EntityTypeBuilder<Integration> builder)
        {
            base.Configure(builder);

            builder.Property(integration => integration.Name)
                .IsRequired()
                .HasMaxLength(256);
            
            builder.Property(integration => integration.ClientDisplayName)
                .IsRequired()
                .HasMaxLength(256);
        }
    }
}
