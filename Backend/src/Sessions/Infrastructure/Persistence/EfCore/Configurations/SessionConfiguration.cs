using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenDDD.Infrastructure.Persistence.EfCore.Base;
using Sessions.Domain.Model;
using Sessions.Domain.Model.BankAccounts;

namespace Sessions.Infrastructure.Persistence.EfCore.Configurations
{
    public class SessionConfiguration : EfAggregateRootConfigurationBase<Session, Guid>
    {
        public override void Configure(EntityTypeBuilder<Session> builder)
        {
            // Call base configuration
            base.Configure(builder);

            // Properties
            builder.Property(session => session.State)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(session => session.Type)
                .IsRequired()
                .HasConversion<string>();
            
            builder.Property(session => session.IsStarted)
                .IsRequired();

            builder.Property(session => session.SelectedBankId)
                .HasMaxLength(128)
                .IsRequired(false);

            builder.Property(session => session.SelectedIntegrationId)
                .HasMaxLength(128)
                .IsRequired(false);
            
            // Ownership: User
            builder.OwnsOne(s => s.User, user =>
            {
                user.Property(u => u.Nin)
                    .IsRequired(false)
                    .HasMaxLength(20);

                user.Property(u => u.Name)
                    .IsRequired(false)
                    .HasMaxLength(128);
            });

            // Ownership: BankAccounts
            builder.Property(session => session.BankAccounts)
                .HasColumnType("TEXT")
                .HasConversion(
                    v => v == null ? "{}" : JsonSerializer.Serialize(v, new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull }),
                    v => string.IsNullOrEmpty(v) ? BankAccounts.NoAccounts() : JsonSerializer.Deserialize<BankAccounts>(v, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                );
            
            // Ownership: TransactionHistory
            builder.Property(session => session.TransactionHistory)
                .HasColumnType("TEXT")
                .HasConversion(
                    v => v == null ? "{}" : JsonSerializer.Serialize(v, new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull }),
                    v => string.IsNullOrEmpty(v) ? TransactionHistory.NoTransactions() : JsonSerializer.Deserialize<TransactionHistory>(v, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                );
        }
    }
}
