using MarketZone.Application.DTOs;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Parameters;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.DTOs;
using MarketZone.Domain.Cash.Entities;
using MarketZone.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

        public async Task<PagedResponse<ExchangeRateDto>> GetPagedListAsync(ExchangeRateFilter filter)
        {
            var query = _context.Set<ExchangeRate>().AsQueryable();

            // Apply filters using FilterBuilder pattern
            if (filter.IsActive.HasValue)
            {
                query = query.Where(r => r.IsActive == filter.IsActive.Value);
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(r => r.EffectiveDate)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(r => new ExchangeRateDto
                {
                    Id = r.Id,
                    Rate = r.Rate,
                    EffectiveDate = r.EffectiveDate,
                    IsActive = r.IsActive
                })
                .ToListAsync();

            return PagedResponse<ExchangeRateDto>.Ok(new PaginationResponseDto<ExchangeRateDto>(items, totalCount, filter.PageNumber, filter.PageSize));
        }
    }
}