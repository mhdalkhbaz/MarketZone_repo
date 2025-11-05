using MarketZone.Domain.Inventory.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketZone.Infrastructure.Persistence.Contexts.Configurations
{
    public class ProductBalanceConfiguration : IEntityTypeConfiguration<ProductBalance>
    {
        public void Configure(EntityTypeBuilder<ProductBalance> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Qty).HasColumnType("decimal(18,2)").HasDefaultValue(0);
            builder.Property(x => x.AvailableQty).HasColumnType("decimal(18,2)").HasDefaultValue(0);
            builder.Property(x => x.TotalValue).HasColumnType("decimal(18,6)").HasDefaultValue(0);
            builder.Property(x => x.AverageCost).HasColumnType("decimal(18,6)").HasDefaultValue(0);

            builder.HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}



