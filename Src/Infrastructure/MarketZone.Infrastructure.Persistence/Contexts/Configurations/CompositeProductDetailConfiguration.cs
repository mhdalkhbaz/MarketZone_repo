using MarketZone.Domain.Products.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketZone.Infrastructure.Persistence.Contexts.Configurations
{
    public class CompositeProductDetailConfiguration : IEntityTypeConfiguration<CompositeProductDetail>
    {
        public void Configure(EntityTypeBuilder<CompositeProductDetail> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Quantity).HasColumnType("decimal(18,2)").IsRequired();

            builder.HasOne(d => d.ComponentProduct)
                .WithMany()
                .HasForeignKey(d => d.ComponentProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

