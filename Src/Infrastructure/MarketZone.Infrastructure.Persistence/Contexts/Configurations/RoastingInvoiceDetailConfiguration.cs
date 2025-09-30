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

            builder.Property(x => x.RawProductId)
                .IsRequired();

            builder.Property(x => x.QuantityKg)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.RoastingCost)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.ReceivedQuantityKg)
                .HasPrecision(18, 2);

            builder.Property(x => x.Notes)
                .HasMaxLength(500);

            // Foreign key relationships
            builder.HasOne(x => x.RoastingInvoice)
                .WithMany(x => x.Details)
                .HasForeignKey(x => x.RoastingInvoiceId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.RawProduct)
                .WithMany()
                .HasForeignKey(x => x.RawProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
