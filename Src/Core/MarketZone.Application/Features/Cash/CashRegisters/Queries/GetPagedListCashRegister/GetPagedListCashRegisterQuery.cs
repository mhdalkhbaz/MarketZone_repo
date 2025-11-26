using MarketZone.Application.Interfaces;
using MarketZone.Application.Parameters;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.DTOs;

namespace MarketZone.Application.Features.Cash.CashRegisters.Queries.GetPagedListCashRegister
{
	public class GetPagedListCashRegisterQuery : CashRegisterFilter, IRequest<PagedResponse<CashRegisterDto>>
	{
	}
}


