using DDD.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DDD.Infrastructure.Repositories.EF.Configurations
{
    public abstract class EfAggregateRootConfigurationBase<TEfAggregateRoot, TId> : IEntityTypeConfiguration<TEfAggregateRoot>
        where TEfAggregateRoot : class, IAggregateRoot<TId>
    {
        public virtual void Configure(EntityTypeBuilder<TEfAggregateRoot> builder)
        {
            // Primary Key
            builder.HasKey("Id");

            // ID Property
            builder.Property(typeof(TId), "Id").IsRequired();
            
            // Timestamp Properties
            builder.Property(typeof(DateTime), "CreatedAt").IsRequired();
            builder.Property(typeof(DateTime), "UpdatedAt").IsRequired();
            
            // Table Name
            builder.ToTable(GetTableName());
        }

        protected abstract string GetTableName();
    }
}
