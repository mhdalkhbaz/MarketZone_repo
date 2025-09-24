using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Logistics.DTOs;

namespace MarketZone.Application.Features.Logistics.Queries.GetRegionById
{
	public class GetRegionByIdQuery : IRequest<BaseResult<RegionDto>>
	{
		public long Id { get; set; }
	}
}


