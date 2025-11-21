using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Logistics.Commands.Regions.DeleteRegion
{
	public class DeleteRegionCommand : IRequest<BaseResult>
	{
		public long Id { get; set; }
	}
}

