using MarketZone.Domain.Sales.Entities;
using MarketZone.Domain.Sales.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketZone.Infrastructure.Persistence.Contexts.Configurations
{
	public class SalesInvoiceConfiguration : IEntityTypeConfiguration<SalesInvoice>
	{
		public void Configure(EntityTypeBuilder<SalesInvoice> builder)
		{
			builder.HasKey(x => x.Id);
			builder.Property(p => p.InvoiceNumber).HasMaxLength(50).IsRequired();
			builder.Property(p => p.InvoiceDate).HasColumnType("datetime2");
			builder.Property(p => p.TotalAmount).HasColumnType("decimal(18,2)");
			builder.Property(p => p.Discount).HasColumnType("decimal(18,2)");
			builder.Property(p => p.PaymentMethod).HasMaxLength(50);
			builder.Property(p => p.Notes).HasMaxLength(255);
			builder.Property(p => p.Status).HasConversion<short>().HasDefaultValue(SalesInvoiceStatus.Draft);
			builder.Property(p => p.Type).HasConversion<short>().HasDefaultValue(SalesInvoiceType.Regular);
			builder.Property(p => p.Currency).HasConversion<short>();

			builder.HasOne(p => p.Customer)
				.WithMany()
				.HasForeignKey(p => p.CustomerId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.HasOne(p => p.DistributionTrip)
				.WithMany(t => t.DistributionTripSalesInvoices)
				.HasForeignKey(p => p.DistributionTripId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.HasMany(p => p.Details)
				.WithOne(d => d.Invoice)
				.HasForeignKey(d => d.InvoiceId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}



