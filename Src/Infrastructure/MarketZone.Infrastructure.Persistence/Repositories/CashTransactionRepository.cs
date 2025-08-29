using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MarketZone.Application.DTOs;
using MarketZone.Application.Interfaces.Repositories;
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
	}
}


