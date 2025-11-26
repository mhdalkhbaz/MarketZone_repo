using MarketZone.Application.DTOs;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Parameters;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.DTOs;
using MarketZone.Domain.Cash.Entities;
using MarketZone.Domain.Cash.Enums;
using MarketZone.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MarketZone.Infrastructure.Persistence.Repositories
{
    public class ExchangeTransactionRepository : GenericRepository<ExchangeTransaction>, IExchangeTransactionRepository
    {
        private readonly ApplicationDbContext _context;

        public ExchangeTransactionRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<ExchangeTransaction>> GetByCashRegisterAsync(long cashRegisterId, CancellationToken cancellationToken)
        {
            return await _context.Set<ExchangeTransaction>()
                .Where(t => t.CashRegisterId == cashRegisterId)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<ExchangeTransaction>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken)
        {
            return await _context.Set<ExchangeTransaction>()
                .Where(t => t.TransactionDate >= fromDate && t.TransactionDate <= toDate)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<ExchangeTransaction>> GetByDirectionAsync(ExchangeDirection direction, CancellationToken cancellationToken)
        {
            return await _context.Set<ExchangeTransaction>()
                .Where(t => t.Direction == direction)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<PagedResponse<ExchangeTransactionDto>> GetPagedListAsync(ExchangeTransactionFilter filter)
        {
            var query = _context.Set<ExchangeTransaction>().AsQueryable();

            // Apply filters using FilterBuilder pattern
            if (filter.CashRegisterId.HasValue)
            {
                query = query.Where(t => t.CashRegisterId == filter.CashRegisterId.Value);
            }

            if (filter.FromDate.HasValue)
            {
                query = query.Where(t => t.TransactionDate >= filter.FromDate.Value);
            }

            if (filter.ToDate.HasValue)
            {
                query = query.Where(t => t.TransactionDate <= filter.ToDate.Value);
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(t => t.TransactionDate)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(t => new ExchangeTransactionDto
                {
                    Id = t.Id,
                    CashRegisterId = t.CashRegisterId,
                    Direction = t.Direction,
                    FromAmount = t.FromAmount,
                    ToAmount = t.ToAmount,
                    ExchangeRate = t.ExchangeRate,
                    TransactionDate = t.TransactionDate,
                    Notes = t.Notes
                })
                .ToListAsync();

            return PagedResponse<ExchangeTransactionDto>.Ok(new PaginationResponseDto<ExchangeTransactionDto>(items, totalCount, filter.PageNumber, filter.PageSize));
        }
    }
}
