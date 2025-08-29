using MarketZone.Domain.Purchases.Entities;
using MarketZone.Domain.Purchases.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketZone.Infrastructure.Persistence.Contexts.Configurations
{
	public class PurchaseInvoiceConfiguration : IEntityTypeConfiguration<PurchaseInvoice>
	{
		public void Configure(EntityTypeBuilder<PurchaseInvoice> builder)
		{
			builder.HasKey(x => x.Id);
			builder.HasIndex(x => x.InvoiceNumber).IsUnique();
			builder.Property(x => x.InvoiceNumber).HasMaxLength(50).IsRequired();
			builder.Property(x => x.InvoiceDate).HasDefaultValueSql("GETUTCDATE()");
			builder.Property(x => x.TotalAmount).HasColumnType("decimal(18,2)").IsRequired();
			builder.Property(x => x.Discount).HasColumnType("decimal(18,2)").HasDefaultValue(0);
			builder.Property(x => x.Notes).HasMaxLength(500);
			builder.Property(x => x.Status)
				.HasConversion<short>()
				.HasDefaultValue(PurchaseInvoiceStatus.Draft)
				.IsRequired();
			builder.Property(x => x.PaymentStatus)
				.HasConversion<short>()
				.HasDefaultValue(PurchasePaymentStatus.InProgress)
				.IsRequired();

			builder.HasMany(x => x.Details)
				.WithOne(d => d.Invoice)
				.HasForeignKey(d => d.InvoiceId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}



