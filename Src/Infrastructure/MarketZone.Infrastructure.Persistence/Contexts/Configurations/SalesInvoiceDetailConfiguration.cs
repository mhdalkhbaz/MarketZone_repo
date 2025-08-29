using MarketZone.Domain.Sales.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketZone.Infrastructure.Persistence.Contexts.Configurations
{
	public class SalesInvoiceDetailConfiguration : IEntityTypeConfiguration<SalesInvoiceDetail>
	{
		public void Configure(EntityTypeBuilder<SalesInvoiceDetail> builder)
		{
			builder.HasKey(x => x.Id);
			builder.Property(x => x.Quantity).HasColumnType("decimal(18,2)").IsRequired();
			builder.Property(x => x.UnitPrice).HasColumnType("decimal(18,2)").IsRequired();
			builder.Property(x => x.SubTotal).HasColumnType("decimal(18,2)").IsRequired();
			builder.Property(x => x.Notes).HasMaxLength(500);
		}
	}
}



