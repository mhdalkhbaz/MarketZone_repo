using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MarketZone.Application.DTOs;
using MarketZone.Application.Interfaces.Repositories;
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
		public async Task<PaginationResponseDto<CashTransactionDto>> GetPagedListAsync(int pageNumber, int pageSize, long? cashRegisterId)
		{
			var query = dbContext.Set<CashTransaction>()
				.OrderByDescending(p => p.TransactionDate)
				.AsQueryable();

			if (cashRegisterId.HasValue)
				query = query.Where(p => p.CashRegisterId == cashRegisterId.Value);

			return await Paged(
				query.ProjectTo<CashTransactionDto>(mapper.ConfigurationProvider),
				pageNumber,
				pageSize);
		}

		public async Task<CashTransaction> GetByReferenceAsync(ReferenceType referenceType, long referenceId, System.Threading.CancellationToken cancellationToken = default)
		{
			return await dbContext.Set<CashTransaction>()
				.FirstOrDefaultAsync(x => x.ReferenceType == referenceType && x.ReferenceId == referenceId, cancellationToken);
		}

		public async Task<PagedResponse<CashTransactionDto>> GetPagedListAsync(int pageNumber, int pageSize, long? cashRegisterId = null, TransactionType? transactionType = null, DateTime? fromDate = null, DateTime? toDate = null, ReferenceType? referenceType = null, long? referenceId = null)
		{
			var query = dbContext.Set<CashTransaction>().AsQueryable();

			if (cashRegisterId.HasValue)
				query = query.Where(p => p.CashRegisterId == cashRegisterId.Value);

			if (transactionType.HasValue)
				query = query.Where(p => p.TransactionType == transactionType.Value);

			if (fromDate.HasValue)
				query = query.Where(p => p.TransactionDate >= fromDate.Value);

			if (toDate.HasValue)
				query = query.Where(p => p.TransactionDate <= toDate.Value);

			if (referenceType.HasValue)
				query = query.Where(p => p.ReferenceType == referenceType.Value);

			if (referenceId.HasValue)
				query = query.Where(p => p.ReferenceId == referenceId.Value);

			var totalCount = await query.CountAsync();
			var items = await query
				.OrderByDescending(p => p.TransactionDate)
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
				.ProjectTo<CashTransactionDto>(mapper.ConfigurationProvider)
				.ToListAsync();

			return PagedResponse<CashTransactionDto>.Ok(new PaginationResponseDto<CashTransactionDto>(items, totalCount, pageNumber, pageSize));
		}

		public async Task<CashTransaction> GetByIdAsync(long id, CancellationToken cancellationToken)
		{
			return await dbContext.Set<CashTransaction>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
		}
	}
}


