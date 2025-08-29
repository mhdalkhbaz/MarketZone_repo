using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.DTOs;

namespace MarketZone.Application.Features.Cash.CashTransactions.Queries.GetPagedListCashTransaction
{
	public class GetPagedListCashTransactionQueryHandler(ICashTransactionRepository repository) : IRequestHandler<GetPagedListCashTransactionQuery, PagedResponse<CashTransactionDto>>
	{
		public async Task<PagedResponse<CashTransactionDto>> Handle(GetPagedListCashTransactionQuery request, CancellationToken cancellationToken)
		{
			var paged = await repository.GetPagedListAsync(request.PageNumber, request.PageSize, request.CashRegisterId);
			return PagedResponse<CashTransactionDto>.Ok(paged);
		}
	}
}


