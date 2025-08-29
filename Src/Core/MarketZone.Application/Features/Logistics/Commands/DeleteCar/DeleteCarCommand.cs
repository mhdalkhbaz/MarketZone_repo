using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Logistics.Commands.DeleteCar
{
	public class DeleteCarCommand : IRequest<BaseResult>
	{
		public long Id { get; set; }
	}
}


