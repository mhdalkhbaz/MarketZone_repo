using MarketZone.Application.DTOs;
using MarketZone.Application.Interfaces.Repositories;
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

        public async Task<PagedResponse<ExchangeTransactionDto>> GetPagedListAsync(int pageNumber, int pageSize, long? cashRegisterId = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _context.Set<ExchangeTransaction>().AsQueryable();

            if (cashRegisterId.HasValue)
            {
                query = query.Where(t => t.CashRegisterId == cashRegisterId.Value);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(t => t.TransactionDate >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(t => t.TransactionDate <= toDate.Value);
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(t => t.TransactionDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
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

            return PagedResponse<ExchangeTransactionDto>.Ok(new PaginationResponseDto<ExchangeTransactionDto>(items, totalCount, pageNumber, pageSize));
        }
    }
}
