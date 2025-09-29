using MarketZone.Domain.Roasting.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketZone.Infrastructure.Persistence.Contexts.Configurations
{
    public class RoastingInvoiceDetailReceiptConfiguration : IEntityTypeConfiguration<RoastingInvoiceDetailReceipt>
    {
        public void Configure(EntityTypeBuilder<RoastingInvoiceDetailReceipt> builder)
        {
            builder.Property(x => x.RoastingInvoiceId)
                .IsRequired();

            builder.Property(x => x.DetailId)
                .IsRequired();

            builder.Property(x => x.ReadyProductId)
                .IsRequired();

            builder.Property(x => x.QuantityKg)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.SalePricePerKg)
                .HasPrecision(18, 4)
                .IsRequired();

            builder.Property(x => x.RoastingCostPerKg)
                .HasPrecision(18, 4)
                .IsRequired();

            builder.Property(x => x.CommissionPerKg)
                .HasPrecision(18, 4)
                .IsRequired();

            builder.Property(x => x.NetSalePricePerKg)
                .HasPrecision(18, 4)
                .IsRequired();

            builder.Ignore(x => x.TotalSalePrice);
            builder.Ignore(x => x.TotalRoastingCost);
            builder.Ignore(x => x.TotalCommission);
            builder.Ignore(x => x.TotalNetSalePrice);

            builder.HasOne(x => x.Detail)
                .WithMany(x => x.Receipts)
                .HasForeignKey(x => x.DetailId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<RoastingInvoice>()
                .WithMany(x => x.Receipts)
                .HasForeignKey(x => x.RoastingInvoiceId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}


