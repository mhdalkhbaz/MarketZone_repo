using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Logistics.Commands.Regions.UpdateRegion
{
	public class UpdateRegionCommand : IRequest<BaseResult>
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public bool? IsActive { get; set; }
	}
}


