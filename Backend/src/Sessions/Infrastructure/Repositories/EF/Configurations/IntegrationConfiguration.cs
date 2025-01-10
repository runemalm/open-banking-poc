using DDD.Infrastructure.Repositories.EF.Configurations;
using Sessions.Domain.Model.Integration;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Sessions.Infrastructure.Repositories.EF.Configurations
{
    public class IntegrationConfiguration : EfAggregateRootConfigurationBase<Integration, Guid>
    {
        protected override string GetTableName()
        {
            return "Integrations";
        }

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
