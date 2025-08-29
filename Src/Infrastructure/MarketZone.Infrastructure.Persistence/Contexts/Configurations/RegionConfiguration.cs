using MarketZone.Domain.Logistics.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketZone.Infrastructure.Persistence.Contexts.Configurations
{
	public class RegionConfiguration : IEntityTypeConfiguration<Region>
	{
		public void Configure(EntityTypeBuilder<Region> builder)
		{
			builder.HasKey(x => x.Id);
			builder.Property(p => p.Name).HasMaxLength(100).IsRequired();
			builder.Property(p => p.Description).HasMaxLength(255);
			builder.Property(p => p.IsActive).HasDefaultValue(true);
		}
	}
}


