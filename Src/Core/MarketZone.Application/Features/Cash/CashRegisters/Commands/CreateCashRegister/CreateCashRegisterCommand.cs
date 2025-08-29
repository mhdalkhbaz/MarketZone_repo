using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Cash.CashRegisters.Commands.CreateCashRegister
{
	public class CreateCashRegisterCommand : IRequest<BaseResult<long>>
	{
		public string Name { get; set; }
		public decimal OpeningBalance { get; set; }
		public decimal OpeningBalanceDollar { get; set; }
	}
}


