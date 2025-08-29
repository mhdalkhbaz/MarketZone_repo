using MarketZone.Domain.Inventory.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketZone.Infrastructure.Persistence.Contexts.Configurations
{
	public class InventoryHistoryConfiguration : IEntityTypeConfiguration<InventoryHistory>
	{
		public void Configure(EntityTypeBuilder<InventoryHistory> builder)
		{
			builder.HasKey(x => x.Id);
			builder.Property(x => x.TransactionType).HasMaxLength(50).IsRequired();
			builder.Property(x => x.Quantity).HasColumnType("decimal(18,2)").IsRequired();
			builder.Property(x => x.Date).HasDefaultValueSql("GETUTCDATE()");
			builder.Property(x => x.Notes).HasMaxLength(500);

			builder.HasOne(x => x.Product)
				.WithMany()
				.HasForeignKey(x => x.ProductId)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}



