using DDD.Infrastructure.Repositories.EF.Configurations;
using Demo.Domain.Model.Bank;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demo.Infrastructure.Repositories.EF.Configurations
{
    public class BankConfiguration : EfAggregateRootConfigurationBase<Bank, Guid>
    {
        protected override string GetTableName()
        {
            return "Banks";
        }

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
