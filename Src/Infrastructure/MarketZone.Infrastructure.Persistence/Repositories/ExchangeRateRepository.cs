using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.Entities;
using MarketZone.Domain.Cash.DTOs;
using MarketZone.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace MarketZone.Infrastructure.Persistence.Repositories
{
    public class ExchangeRateRepository : GenericRepository<ExchangeRate>, IExchangeRateRepository
    {
        private readonly ApplicationDbContext _context;

        public ExchangeRateRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ExchangeRate> GetLatestActiveRateAsync(CancellationToken cancellationToken)
        {
            return await _context.Set<ExchangeRate>()
                .Where(r => r.IsActive)
                .OrderByDescending(r => r.EffectiveDate)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<List<ExchangeRate>> GetActiveRatesAsync(CancellationToken cancellationToken)
        {
            return await _context.Set<ExchangeRate>()
                .Where(r => r.IsActive)
                .OrderByDescending(r => r.EffectiveDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<ExchangeRate>> GetRatesByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken)
        {
            return await _context.Set<ExchangeRate>()
                .Where(r => r.EffectiveDate >= fromDate && r.EffectiveDate <= toDate)
                .OrderByDescending(r => r.EffectiveDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<PagedResponse<ExchangeRateDto>> GetPagedListAsync(int pageNumber, int pageSize, bool? isActive = null)
        {
            var query = _context.Set<ExchangeRate>().AsQueryable();

            if (isActive.HasValue)
            {
                query = query.Where(r => r.IsActive == isActive.Value);
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(r => r.EffectiveDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new ExchangeRateDto
                {
                    Id = r.Id,
                    Rate = r.Rate,
                    EffectiveDate = r.EffectiveDate,
                    IsActive = r.IsActive
                })
                .ToListAsync();

            return PagedResponse<ExchangeRateDto>.Ok(items, totalCount, pageNumber, pageSize);
        }
    }
}