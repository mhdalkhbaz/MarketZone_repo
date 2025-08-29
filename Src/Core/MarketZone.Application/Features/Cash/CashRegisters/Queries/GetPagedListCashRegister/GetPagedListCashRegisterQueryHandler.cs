using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.DTOs;

namespace MarketZone.Application.Features.Cash.CashRegisters.Queries.GetPagedListCashRegister
{
	public class GetPagedListCashRegisterQueryHandler(ICashRegisterRepository repository) : IRequestHandler<GetPagedListCashRegisterQuery, PagedResponse<CashRegisterDto>>
	{
		public async Task<PagedResponse<CashRegisterDto>> Handle(GetPagedListCashRegisterQuery request, CancellationToken cancellationToken)
		{
			var paged = await repository.GetPagedListAsync(request.PageNumber, request.PageSize, request.Name);
			return PagedResponse<CashRegisterDto>.Ok(paged);
		}
	}
}


