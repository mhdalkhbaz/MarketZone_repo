using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.DTOs;

namespace MarketZone.Application.Features.Cash.CashRegisters.Queries.GetPagedListCashRegister
{
	public class GetPagedListCashRegisterQuery : IRequest<PagedResponse<CashRegisterDto>>
	{
		public int PageNumber { get; set; } = 1;
		public int PageSize { get; set; } = 10;
		public string Name { get; set; }
	}
}


