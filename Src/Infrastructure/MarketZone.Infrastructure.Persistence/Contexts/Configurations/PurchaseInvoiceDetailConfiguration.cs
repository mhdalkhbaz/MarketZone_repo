using MarketZone.Domain.Purchases.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketZone.Infrastructure.Persistence.Contexts.Configurations
{
    public class PurchaseInvoiceDetailConfiguration : IEntityTypeConfiguration<PurchaseInvoiceDetail>
    {
        public void Configure(EntityTypeBuilder<PurchaseInvoiceDetail> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Quantity).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(x => x.UnitPrice).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(x => x.TotalPrice).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(x => x.Notes).HasMaxLength(500);
        }
    }
}



