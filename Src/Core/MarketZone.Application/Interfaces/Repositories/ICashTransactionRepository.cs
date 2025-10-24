using System;
using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.DTOs;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.DTOs;
using MarketZone.Domain.Cash.Entities;
using MarketZone.Domain.Cash.Enums;

namespace MarketZone.Application.Interfaces.Repositories
{
	public interface ICashTransactionRepository : IGenericRepository<CashTransaction>
	{
		Task<PaginationResponseDto<CashTransactionDto>> GetPagedListAsync(int pageNumber, int pageSize, long? cashRegisterId);
		Task<PagedResponse<CashTransactionDto>> GetPagedListAsync(int pageNumber, int pageSize, long? cashRegisterId = null, TransactionType? transactionType = null, DateTime? fromDate = null, DateTime? toDate = null, ReferenceType? referenceType = null, long? referenceId = null);
		Task<CashTransaction> GetByReferenceAsync(ReferenceType referenceType, long referenceId, System.Threading.CancellationToken cancellationToken = default);
		Task<CashTransaction> GetByIdAsync(long id, CancellationToken cancellationToken);
	}
}


