using MarketZone.Domain.Inventory.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketZone.Infrastructure.Persistence.Contexts.Configurations
{
	public class UnroastedProdcutBalanceConfiguration : IEntityTypeConfiguration<UnroastedProdcutBalance>
	{
		public void Configure(EntityTypeBuilder<UnroastedProdcutBalance> builder)
		{
			builder.HasKey(x => x.Id);
			builder.HasIndex(x => x.ProductId).IsUnique();
			builder.Property(x => x.Qty).HasColumnType("decimal(18,6)").HasDefaultValue(0);
			builder.Property(x => x.AvailableQty).HasColumnType("decimal(18,6)").HasDefaultValue(0);
			builder.Property(x => x.TotalValue).HasColumnType("decimal(18,6)").HasDefaultValue(0);
		}
	}
}


