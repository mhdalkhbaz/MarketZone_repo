using System.Threading.Tasks;
using MarketZone.Application.DTOs;
using MarketZone.Domain.Cash.DTOs;
using MarketZone.Domain.Cash.Entities;
using MarketZone.Domain.Cash.Enums;

namespace MarketZone.Application.Interfaces.Repositories
{
	public interface ICashTransactionRepository : IGenericRepository<CashTransaction>
	{
		Task<PaginationResponseDto<CashTransactionDto>> GetPagedListAsync(int pageNumber, int pageSize, long? cashRegisterId);
		Task<CashTransaction> GetByReferenceAsync(ReferenceType referenceType, long referenceId, System.Threading.CancellationToken cancellationToken = default);
	}
}


