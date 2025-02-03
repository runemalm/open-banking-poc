using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenDDD.Infrastructure.Persistence.EfCore.Base;
using Sessions.Domain.Model.Bank;

namespace Sessions.Infrastructure.Persistence.EfCore.Configurations
{
    public class BankConfiguration : EfAggregateRootConfigurationBase<Bank, Guid>
    {
        public override void Configure(EntityTypeBuilder<Bank> builder)
        {
            base.Configure(builder);

            builder.Property(bank => bank.Name)
                .IsRequired()
                .HasMaxLength(256);
            
            builder.HasMany(b => b.Integrations)
                .WithOne(i => i.Bank)
                .HasForeignKey(i => i.BankId)
                .IsRequired();
        }
    }
}
