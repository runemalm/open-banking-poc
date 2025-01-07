using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DDD.Infrastructure.Repositories.EF.Configurations;
using Demo.Domain.Model;

namespace Demo.Infrastructure.Repositories.EF.Configurations
{
    public class SessionConfiguration : EfAggregateRootConfigurationBase<Session, Guid>
    {
        protected override string GetTableName()
        {
            return "Sessions";
        }

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
            builder.OwnsOne(s => s.BankAccounts, bankAccounts =>
            {
                bankAccounts.OwnsMany(b => b.Accounts, account =>
                {
                    account.Property(a => a.Number)
                        .IsRequired()
                        .HasMaxLength(30);

                    account.WithOwner();
                    
                    account.ToTable("BankAccounts");
                });
            });

            // Ownership: TransactionHistory
            builder.OwnsOne(s => s.TransactionHistory, transactionHistory =>
            {
                transactionHistory.OwnsMany(th => th.Transactions, transaction =>
                {
                    transaction.Property(t => t.Description)
                        .IsRequired()
                        .HasMaxLength(255);

                    transaction.Property(t => t.Date)
                        .IsRequired();

                    transaction.Property(t => t.Type)
                        .IsRequired();

                    // Ownership: Money (inside Transaction)
                    transaction.OwnsOne(t => t.Amount, money =>
                    {
                        money.Property(m => m.Amount)
                            .IsRequired();

                        money.Property(m => m.Currency)
                            .IsRequired();
                    });

                    transaction.WithOwner();
                    
                    transaction.ToTable("Transactions");
                });
            });
        }
    }
}
