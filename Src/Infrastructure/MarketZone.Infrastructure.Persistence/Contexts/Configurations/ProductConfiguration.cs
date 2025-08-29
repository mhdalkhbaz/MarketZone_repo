using MarketZone.Domain.Products.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketZone.Infrastructure.Persistence.Contexts.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {

        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(p => p.Name).HasMaxLength(100).IsRequired();
            builder.Property(p => p.Description).HasMaxLength(500);
            builder.Property(p => p.UnitOfMeasure).HasMaxLength(100).HasDefaultValue("kg");
            builder.Property(p => p.PurchasePrice).HasColumnType("decimal(18,2)");
            builder.Property(p => p.SalePrice).HasColumnType("decimal(18,2)");
            builder.Property(p => p.MinStockLevel).HasColumnType("decimal(18,2)").HasDefaultValue(5);
            builder.Property(p => p.IsActive).HasDefaultValue(true);
            builder.Property(p => p.NeedsRoasting).HasDefaultValue(false);
            builder.Property(p => p.RoastingCost).HasColumnType("decimal(18,2)");
            builder.Property(p => p.BarCode).HasMaxLength(50);

            builder.HasOne(p => p.Category)
                .WithMany()
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
