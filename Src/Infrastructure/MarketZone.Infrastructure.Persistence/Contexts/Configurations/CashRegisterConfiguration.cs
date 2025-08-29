using MarketZone.Domain.Cash.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketZone.Infrastructure.Persistence.Contexts.Configurations
{
	public class CashRegisterConfiguration : IEntityTypeConfiguration<CashRegister>
	{
		public void Configure(EntityTypeBuilder<CashRegister> builder)
		{
			builder.HasKey(x => x.Id);
			builder.Property(p => p.Name).HasMaxLength(100).IsRequired();
			builder.Property(p => p.OpeningBalance).HasColumnType("decimal(18,2)").HasDefaultValue(0);
			builder.Property(p => p.OpeningBalanceDollar).HasColumnType("decimal(18,2)").HasDefaultValue(0);
			builder.Property(p => p.CurrentBalance).HasColumnType("decimal(18,2)").HasDefaultValue(0);
			builder.Property(p => p.CurrentBalanceDollar).HasColumnType("decimal(18,2)").HasDefaultValue(0);
		}
	}
}


