using MarketZone.Domain.Roasting.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketZone.Infrastructure.Persistence.Contexts.Configurations
{
	public class RoastingOperationConfiguration : IEntityTypeConfiguration<RoastingOperation>
	{
		public void Configure(EntityTypeBuilder<RoastingOperation> builder)
		{
			builder.HasKey(x => x.Id);
			builder.Property(p => p.QuantityKg).HasColumnType("decimal(18,2)").IsRequired();
			builder.Property(p => p.RoastPricePerKg).HasColumnType("decimal(18,2)").IsRequired();
			builder.Property(p => p.TotalCost).HasColumnType("decimal(18,2)").IsRequired();
			builder.Property(p => p.RoastDate).HasColumnType("datetime2").HasDefaultValueSql("GETUTCDATE()");
			builder.Property(p => p.Notes).HasMaxLength(500);
		}
	}
}


