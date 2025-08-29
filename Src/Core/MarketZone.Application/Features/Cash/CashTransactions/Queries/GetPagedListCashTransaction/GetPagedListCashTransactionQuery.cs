using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.DTOs;

namespace MarketZone.Application.Features.Cash.CashTransactions.Queries.GetPagedListCashTransaction
{
	public class GetPagedListCashTransactionQuery : IRequest<PagedResponse<CashTransactionDto>>
	{
		public int PageNumber { get; set; } = 1;
		public int PageSize { get; set; } = 10;
		public long? CashRegisterId { get; set; }
	}
}


