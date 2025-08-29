using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Logistics.Commands.Regions.CreateRegion
{
	public class CreateRegionCommand : IRequest<BaseResult<long>>
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public bool IsActive { get; set; } = true;
	}
}


