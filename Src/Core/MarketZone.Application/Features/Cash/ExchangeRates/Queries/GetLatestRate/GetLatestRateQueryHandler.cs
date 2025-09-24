using System;
using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.Entities;

namespace MarketZone.Application.Features.Cash.ExchangeRates.Queries.GetLatestRate
{
	public class GetLatestRateQueryHandler : IRequestHandler<GetLatestRateQuery, BaseResult<ExchangeRate>>
	{
		private readonly IExchangeRateRepository _repository;

		public GetLatestRateQueryHandler(IExchangeRateRepository repository)
		{
			_repository = repository;
		}

		public async Task<BaseResult<ExchangeRate>> Handle(GetLatestRateQuery request, CancellationToken cancellationToken)
		{
			var at = request.AtUtc ?? DateTime.UtcNow;
			var rate = await _repository.GetLatestAsync(request.CurrencyCode, at, cancellationToken);
			if (rate == null) return new Error(ErrorCode.NotFound, "Exchange rate not found", nameof(request.CurrencyCode));
			return rate;
		}
	}
}


