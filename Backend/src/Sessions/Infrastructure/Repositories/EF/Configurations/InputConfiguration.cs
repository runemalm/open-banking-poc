using DDD.Infrastructure.Repositories.EF.Configurations;
using Sessions.Domain.Model.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Sessions.Infrastructure.Repositories.EF.Configurations
{
    public class InputConfiguration : EfAggregateRootConfigurationBase<Input, Guid>
    {
        protected override string GetTableName()
        {
            return "Inputs";
        }

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

            builder.Property(input => input.ErrorMessage)
                .HasMaxLength(512)
                .IsRequired(false);

            // Handle Dictionary<string, string> with a JSON conversion
            builder.Property(input => input.RequestParams)
                .HasColumnType("jsonb")
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions)null),
                    v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string?>>(v, (System.Text.Json.JsonSerializerOptions)null)
                );

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
