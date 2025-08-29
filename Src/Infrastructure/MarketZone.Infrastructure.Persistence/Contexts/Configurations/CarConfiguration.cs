using MarketZone.Domain.Logistics.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketZone.Infrastructure.Persistence.Contexts.Configurations
{
	public class CarConfiguration : IEntityTypeConfiguration<Car>
	{
		public void Configure(EntityTypeBuilder<Car> builder)
		{
			builder.HasKey(x => x.Id);
			builder.Property(p => p.PlateNumber).HasMaxLength(50);
			builder.Property(p => p.Name).HasMaxLength(100).IsRequired();
			builder.Property(p => p.Model).HasMaxLength(100);
			builder.Property(p => p.Year);
			builder.Property(p => p.CapacityKg).HasColumnType("decimal(18,2)");
			builder.Property(p => p.IsAvailable).HasDefaultValue(true);
		}
	}
}


