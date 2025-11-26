using MarketZone.Application.Interfaces;
using MarketZone.Application.Parameters;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Logistics.DTOs;

namespace MarketZone.Application.Features.Logistics.Queries.GetPagedListRegion
{
	public class GetPagedListRegionQuery : RegionFilter, IRequest<PagedResponse<RegionDto>>
	{
	}
}


