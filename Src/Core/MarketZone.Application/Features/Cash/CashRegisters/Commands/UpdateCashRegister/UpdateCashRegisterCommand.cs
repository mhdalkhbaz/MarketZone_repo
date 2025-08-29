using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Cash.CashRegisters.Commands.UpdateCashRegister
{
	public class UpdateCashRegisterCommand : IRequest<BaseResult>
	{
		public long Id { get; set; }
		public string Name { get; set; }
	}
}


