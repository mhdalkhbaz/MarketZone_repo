using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Logistics.DTOs;

namespace MarketZone.Application.Features.Logistics.Queries.GetPagedListDistributionTrip
{
	public class GetPagedListDistributionTripQuery : IRequest<PagedResponse<DistributionTripDto>>
	{
		public int PageNumber { get; set; } = 1;
		public int PageSize { get; set; } = 10;
		public long? CarId { get; set; }
		public long? RegionId { get; set; }
	}
}


