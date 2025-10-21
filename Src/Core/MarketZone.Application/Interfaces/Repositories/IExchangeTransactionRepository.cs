using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MarketZone.Domain.Cash.Entities;
using MarketZone.Domain.Cash.Enums;

namespace MarketZone.Application.Interfaces.Repositories
{
    public interface IExchangeTransactionRepository : IGenericRepository<ExchangeTransaction>
    {
        Task<List<ExchangeTransaction>> GetByCashRegisterAsync(long cashRegisterId, CancellationToken cancellationToken);
        Task<List<ExchangeTransaction>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken);
        Task<List<ExchangeTransaction>> GetByDirectionAsync(ExchangeDirection direction, CancellationToken cancellationToken);
    }
}
