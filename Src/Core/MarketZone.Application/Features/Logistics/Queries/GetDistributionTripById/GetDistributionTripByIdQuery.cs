using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Logistics.DTOs;

namespace MarketZone.Application.Features.Logistics.Queries.GetDistributionTripById
{
	public class GetDistributionTripByIdQuery : IRequest<BaseResult<DistributionTripDto>>
	{
		public long Id { get; set; }
	}
}








