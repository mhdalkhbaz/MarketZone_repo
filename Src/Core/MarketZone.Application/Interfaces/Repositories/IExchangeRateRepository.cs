using System;
using System.Threading;
using System.Threading.Tasks;
using MarketZone.Domain.Cash.Entities;

namespace MarketZone.Application.Interfaces.Repositories
{
	public interface IExchangeRateRepository : IGenericRepository<ExchangeRate>
	{
		Task<ExchangeRate> GetLatestAsync(string currencyCode, DateTime atUtc, CancellationToken cancellationToken = default);
	}
}


