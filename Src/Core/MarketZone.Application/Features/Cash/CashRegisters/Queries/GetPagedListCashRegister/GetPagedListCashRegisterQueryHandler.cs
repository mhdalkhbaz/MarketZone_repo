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
			var result = await repository.GetPagedListAsync(request);
			return PagedResponse<CashRegisterDto>.Ok(result);
		}
	}
}


