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
			builder.Property(p => p.Qty).HasColumnType("decimal(18,2)").IsRequired();
			builder.Property(p => p.ExpectedPrice).HasColumnType("decimal(18,2)").IsRequired();

			builder.HasOne(d => d.Product)
				.WithMany()
				.HasForeignKey(d => d.ProductId);
		}
	}
}


