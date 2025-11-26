using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MarketZone.Application.DTOs;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Parameters;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.DTOs;
using MarketZone.Domain.Cash.Entities;
using MarketZone.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using MarketZone.Domain.Cash.Enums;

namespace MarketZone.Infrastructure.Persistence.Repositories
{
	public class CashTransactionRepository(ApplicationDbContext dbContext, IMapper mapper) : GenericRepository<CashTransaction>(dbContext), ICashTransactionRepository
	{
		public async Task<CashTransaction> GetByReferenceAsync(ReferenceType referenceType, long referenceId, System.Threading.CancellationToken cancellationToken = default)
		{
			return await dbContext.Set<CashTransaction>()
				.FirstOrDefaultAsync(x => x.ReferenceType == referenceType && x.ReferenceId == referenceId, cancellationToken);
		}

		public async Task<PagedResponse<CashTransactionDto>> GetPagedListAsync(CashTransactionFilter filter)
		{
			var query = dbContext.Set<CashTransaction>().AsQueryable();

			// Apply filters using FilterBuilder pattern
			if (filter.CashRegisterId.HasValue)
				query = query.Where(p => p.CashRegisterId == filter.CashRegisterId.Value);

			if (filter.TransactionType.HasValue)
				query = query.Where(p => p.TransactionType == filter.TransactionType.Value);

			if (filter.FromDate.HasValue)
				query = query.Where(p => p.TransactionDate >= filter.FromDate.Value);

			if (filter.ToDate.HasValue)
				query = query.Where(p => p.TransactionDate <= filter.ToDate.Value);

			if (filter.ReferenceType.HasValue)
				query = query.Where(p => p.ReferenceType == filter.ReferenceType.Value);

			if (filter.ReferenceId.HasValue)
				query = query.Where(p => p.ReferenceId == filter.ReferenceId.Value);

			var totalCount = await query.CountAsync();
			var items = await query
				.OrderByDescending(p => p.TransactionDate)
				.Skip((filter.PageNumber - 1) * filter.PageSize)
				.Take(filter.PageSize)
				.ProjectTo<CashTransactionDto>(mapper.ConfigurationProvider)
				.ToListAsync();

			return PagedResponse<CashTransactionDto>.Ok(new PaginationResponseDto<CashTransactionDto>(items, totalCount, filter.PageNumber, filter.PageSize));
		}

		public async Task<CashTransaction> GetByIdAsync(long id, CancellationToken cancellationToken)
		{
			return await dbContext.Set<CashTransaction>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
		}
	}
}


