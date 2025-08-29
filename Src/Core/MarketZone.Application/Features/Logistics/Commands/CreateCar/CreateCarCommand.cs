using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Logistics.Commands.CreateCar
{
	public class CreateCarCommand : IRequest<BaseResult<long>>
	{
		public string Name { get; set; }
		public string PlateNumber { get; set; }
		public string Model { get; set; }
		public int? Year { get; set; }
		public decimal? CapacityKg { get; set; }
		public bool IsAvailable { get; set; } = true;
	}
}


