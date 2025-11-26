using MarketZone.Application.Interfaces;
using MarketZone.Application.Parameters;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.DTOs;

namespace MarketZone.Application.Features.Cash.CashTransactions.Queries.GetPagedListCashTransaction
{
	public class GetPagedListCashTransactionQuery : CashTransactionFilter, IRequest<PagedResponse<CashTransactionDto>>
	{
	}
}


