using MarketZone.Domain.Cash.Entities;
using MarketZone.Domain.Cash.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketZone.Infrastructure.Persistence.Contexts.Configurations
{
	public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
	{
		public void Configure(EntityTypeBuilder<Payment> builder)
		{
			builder.HasKey(x => x.Id);
			builder.Property(p => p.PaymentType).HasConversion<short>().IsRequired();
			builder.Property(p => p.PaymentDate).HasDefaultValueSql("GETUTCDATE()");
			builder.Property(p => p.Amount).HasColumnType("decimal(18,2)").IsRequired();
			builder.Property(p => p.Notes).HasMaxLength(500);
			builder.Property(p => p.Description).HasMaxLength(500);
			builder.Property(p => p.IsConfirmed).HasDefaultValue(true);
			builder.Property(p => p.Status).HasConversion<short>().HasDefaultValue(PaymentStatus.Draft);
			builder.Property(p => p.Currency).HasConversion<short>();
			builder.Property(p => p.PaymentCurrency).HasConversion<short>();
		}
	}
}


