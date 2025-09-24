using System;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Cash.ExchangeRates.Commands.CreateExchangeRate
{
	public class CreateExchangeRateCommand : IRequest<BaseResult<long>>
	{
		public string CurrencyCode { get; set; }
		public decimal RateToUSD { get; set; }
		public DateTime EffectiveAtUtc { get; set; }
		public string Source { get; set; }
		public string Notes { get; set; }
	}
}


