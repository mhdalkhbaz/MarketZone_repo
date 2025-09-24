using System;
using MarketZone.Domain.Common;

namespace MarketZone.Domain.Cash.Entities
{
	public class ExchangeRate : AuditableBaseEntity
	{
		private ExchangeRate()
		{
		}

		public ExchangeRate(string currencyCode, decimal rateToUSD, DateTime effectiveAtUtc, string source, string notes)
		{
			CurrencyCode = currencyCode?.Trim().ToUpperInvariant() ?? string.Empty;
			RateToUSD = rateToUSD;
			EffectiveAtUtc = DateTime.SpecifyKind(effectiveAtUtc, DateTimeKind.Utc);
			Source = source ?? string.Empty;
			Notes = notes ?? string.Empty;
		}

		public string CurrencyCode { get; private set; }
		public decimal RateToUSD { get; private set; }
		public DateTime EffectiveAtUtc { get; private set; }
		public string Source { get; private set; }
		public string Notes { get; private set; }

		public void Update(string currencyCode, decimal? rateToUSD, DateTime? effectiveAtUtc, string source, string notes)
		{
			if (!string.IsNullOrWhiteSpace(currencyCode))
				CurrencyCode = currencyCode.Trim().ToUpperInvariant();
			if (rateToUSD.HasValue)
				RateToUSD = rateToUSD.Value;
			if (effectiveAtUtc.HasValue)
				EffectiveAtUtc = DateTime.SpecifyKind(effectiveAtUtc.Value, DateTimeKind.Utc);
			if (source != null)
				Source = source;
			if (notes != null)
				Notes = notes;
		}
	}
}


