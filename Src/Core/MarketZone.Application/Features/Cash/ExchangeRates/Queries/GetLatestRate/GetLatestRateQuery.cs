using System;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.Entities;

namespace MarketZone.Application.Features.Cash.ExchangeRates.Queries.GetLatestRate
{
	public class GetLatestRateQuery : IRequest<BaseResult<ExchangeRate>>
	{
		public string CurrencyCode { get; set; }
		public DateTime? AtUtc { get; set; }
	}
}


