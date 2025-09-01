using MarketZone.Domain.Logistics.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketZone.Infrastructure.Persistence.Contexts.Configurations
{
	public class DistributionTripDetailConfiguration : IEntityTypeConfiguration<DistributionTripDetail>
	{
		public void Configure(EntityTypeBuilder<DistributionTripDetail> builder)
		{
			builder.HasKey(x => x.Id);
			builder.Property(x => x.Qty).HasColumnType("decimal(18,2)").IsRequired();
			builder.Property(x => x.ExpectedPrice).HasColumnType("decimal(18,2)").IsRequired();
			builder.Property(x => x.ReturnedQty).HasColumnType("decimal(18,2)").HasDefaultValue(0);
			builder.Property(x => x.SoldQty).HasColumnType("decimal(18,2)").HasDefaultValue(0);

			builder.HasOne(x => x.Trip)
				.WithMany(x => x.Details)
				.HasForeignKey(x => x.TripId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.HasOne(x => x.Product)
				.WithMany()
				.HasForeignKey(x => x.ProductId)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}


