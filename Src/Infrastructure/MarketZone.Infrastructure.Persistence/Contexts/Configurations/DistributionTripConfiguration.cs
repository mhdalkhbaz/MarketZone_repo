using MarketZone.Domain.Logistics.Entities;
using MarketZone.Domain.Logistics.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketZone.Infrastructure.Persistence.Contexts.Configurations
{
	public class DistributionTripConfiguration : IEntityTypeConfiguration<DistributionTrip>
	{
		public void Configure(EntityTypeBuilder<DistributionTrip> builder)
		{
			builder.HasKey(x => x.Id);
			builder.Property(p => p.TripDate).HasColumnType("datetime2");
			builder.Property(p => p.LoadQty).HasColumnType("decimal(18,2)");
			builder.Property(p => p.Notes).HasMaxLength(255);
			builder.Property(p => p.Status).HasConversion<short>().HasDefaultValue(DistributionTripStatus.Draft);

			builder.HasOne(p => p.Car)
				.WithMany()
				.HasForeignKey(p => p.CarId);

			builder.HasOne(p => p.Employee)
				.WithMany()
				.HasForeignKey(p => p.EmployeeId);

			builder.HasOne(p => p.Region)
				.WithMany()
				.HasForeignKey(p => p.RegionId);

			builder.HasMany(p => p.Details)
				.WithOne(d => d.Trip)
				.HasForeignKey(d => d.TripId)
				.OnDelete(DeleteBehavior.Cascade);

			// Configure the relationship with sales invoices
			builder.HasMany(p => p.DistributionTripSalesInvoices)
				.WithOne(i => i.DistributionTrip)
				.HasForeignKey(i => i.DistributionTripId)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}


