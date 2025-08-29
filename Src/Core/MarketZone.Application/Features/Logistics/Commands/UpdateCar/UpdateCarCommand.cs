using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Logistics.Commands.UpdateCar
{
	public class UpdateCarCommand : IRequest<BaseResult>
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public string PlateNumber { get; set; }
		public string Model { get; set; }
		public int? Year { get; set; }
		public decimal? CapacityKg { get; set; }
		public bool? IsAvailable { get; set; }
	}
}


