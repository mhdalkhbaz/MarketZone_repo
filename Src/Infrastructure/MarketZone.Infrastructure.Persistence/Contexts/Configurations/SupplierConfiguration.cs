using MarketZone.Domain.Suppliers.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketZone.Infrastructure.Persistence.Contexts.Configurations
{
	public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
	{
		public void Configure(EntityTypeBuilder<Supplier> builder)
		{
			builder.HasKey(x => x.Id);
			builder.Property(p => p.Name).HasMaxLength(100).IsRequired();
			builder.Property(p => p.Phone).HasMaxLength(50);
			builder.Property(p => p.WhatsAppPhone).HasMaxLength(50);
			builder.Property(p => p.Email).HasMaxLength(100);
			builder.Property(p => p.Address).HasMaxLength(500);
			builder.Property(p => p.IsActive).HasDefaultValue(true);
		}
	}
}



