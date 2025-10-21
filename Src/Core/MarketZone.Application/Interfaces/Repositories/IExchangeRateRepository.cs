using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MarketZone.Domain.Cash.Entities;

namespace MarketZone.Application.Interfaces.Repositories
{
    public interface IExchangeRateRepository : IGenericRepository<ExchangeRate>
    {
        Task<ExchangeRate> GetLatestActiveRateAsync(CancellationToken cancellationToken);
        Task<List<ExchangeRate>> GetActiveRatesAsync(CancellationToken cancellationToken);
        Task<List<ExchangeRate>> GetRatesByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken);
    }
}