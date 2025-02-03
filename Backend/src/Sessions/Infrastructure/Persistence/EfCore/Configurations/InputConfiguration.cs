using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenDDD.Infrastructure.Persistence.EfCore.Base;
using Sessions.Domain.Model.Input;

namespace Sessions.Infrastructure.Persistence.EfCore.Configurations
{
    public class InputConfiguration : EfAggregateRootConfigurationBase<Input, Guid>
    {
        public override void Configure(EntityTypeBuilder<Input> builder)
        {
            base.Configure(builder);

            // Properties
            builder.Property(input => input.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(input => input.RequestedAt)
                .IsRequired(false);

            builder.Property(input => input.RequestType)
                .IsRequired(false)
                .HasConversion<string>();

            builder.Property(input => input.Attempt)
                .IsRequired(false);

            builder.Property(input => input.ProvidedAt)
                .IsRequired(false);

            builder.Property(input => input.Value)
                .HasMaxLength(512)
                .IsRequired(false);

            builder.Property(input => input.RequestParams)
                .HasColumnType("TEXT")
                .HasConversion(
                    v => v.ToString(),
                    v => string.IsNullOrEmpty(v) ? new RequestParams() : RequestParams.FromJson(v)
                )
                .IsRequired(false);

            // Owned entities
            builder.OwnsOne(input => input.Error, owned =>
            {
                owned.Property(e => e.Type)
                    .HasConversion<string>()
                    .IsRequired();

                owned.Property(e => e.Message)
                    .HasMaxLength(512)
                    .IsRequired(false);
            });
        }
    }
}
