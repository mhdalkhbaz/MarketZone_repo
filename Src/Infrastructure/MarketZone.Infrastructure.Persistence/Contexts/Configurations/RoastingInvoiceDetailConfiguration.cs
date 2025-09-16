using MarketZone.Domain.Roasting.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketZone.Infrastructure.Persistence.Contexts.Configurations
{
    public class RoastingInvoiceDetailConfiguration : IEntityTypeConfiguration<RoastingInvoiceDetail>
    {
        public void Configure(EntityTypeBuilder<RoastingInvoiceDetail> builder)
        {
            builder.Property(x => x.RoastingInvoiceId)
                .IsRequired();

            builder.Property(x => x.ReadyProductId)
                .IsRequired();

            builder.Property(x => x.RawProductId);

            builder.Property(x => x.QuantityKg)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.RoastPricePerKg)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.CommissionPerKg)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.TotalPrice)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.ActualQuantityAfterRoasting)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.Notes)
                .HasMaxLength(500);

            // Foreign key relationships
            builder.HasOne(x => x.RoastingInvoice)
                .WithMany(x => x.Details)
                .HasForeignKey(x => x.RoastingInvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.ReadyProduct)
                .WithMany()
                .HasForeignKey(x => x.ReadyProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.RawProduct)
                .WithMany()
                .HasForeignKey(x => x.RawProductId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
