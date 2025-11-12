using MarketZone.Domain.Cash.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketZone.Infrastructure.Persistence.Contexts.Configurations
{
	public class CashTransactionConfiguration : IEntityTypeConfiguration<CashTransaction>
	{
		public void Configure(EntityTypeBuilder<CashTransaction> builder)
		{
			builder.HasKey(x => x.Id);
			builder.Property(p => p.TransactionType).HasConversion<short>().IsRequired();
			builder.Property(p => p.Amount).HasColumnType("decimal(18,2)").IsRequired();
			builder.Property(p => p.Currency).HasConversion<short>().IsRequired();
			builder.Property(p => p.TransactionDate).HasDefaultValueSql("GETUTCDATE()");
			builder.Property(p => p.ReferenceType).HasConversion<short>();
			builder.Property(p => p.Description).HasMaxLength(500);
		}
	}
}


