using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.Entities;
using MarketZone.Domain.Cash.Enums;
using MarketZone.Domain.Cash.DTOs;

namespace MarketZone.Application.Interfaces.Repositories
{
    public interface IExchangeTransactionRepository : IGenericRepository<ExchangeTransaction>
    {
        Task<List<ExchangeTransaction>> GetByCashRegisterAsync(long cashRegisterId, CancellationToken cancellationToken);
        Task<List<ExchangeTransaction>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken);
        Task<List<ExchangeTransaction>> GetByDirectionAsync(ExchangeDirection direction, CancellationToken cancellationToken);
        Task<PagedResponse<ExchangeTransactionDto>> GetPagedListAsync(int pageNumber, int pageSize, long? cashRegisterId = null, DateTime? fromDate = null, DateTime? toDate = null);
    }
}
