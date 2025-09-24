using MarketZone.Domain.Cash.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketZone.Infrastructure.Persistence.Contexts.Configurations
{
	public class ExchangeRateConfiguration : IEntityTypeConfiguration<ExchangeRate>
	{
		public void Configure(EntityTypeBuilder<ExchangeRate> builder)
		{
			builder.Property(x => x.CurrencyCode)
				.IsRequired()
				.HasMaxLength(8);

			builder.Property(x => x.RateToUSD)
				.HasColumnType("decimal(18,3)")
				.IsRequired();

			builder.Property(x => x.EffectiveAtUtc)
				.IsRequired();

			builder.HasIndex(x => new { x.CurrencyCode, x.EffectiveAtUtc }).IsUnique();
		}
	}
}


