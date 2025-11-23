using MarketZone.Domain.Products.Entities;
using MarketZone.Domain.Products.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketZone.Infrastructure.Persistence.Contexts.Configurations
{
    public class CompositeProductConfiguration : IEntityTypeConfiguration<CompositeProduct>
    {
        public void Configure(EntityTypeBuilder<CompositeProduct> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(p => p.SalePrice).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(p => p.CommissionPerKg).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(p => p.Status).HasConversion<short>().HasDefaultValue(CompositeProductStatus.Draft);

            builder.HasOne(p => p.ResultingProduct)
                .WithMany()
                .HasForeignKey(p => p.ResultingProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.Details)
                .WithOne(d => d.CompositeProduct)
                .HasForeignKey(d => d.CompositeProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

