using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Logistics.Commands.DistributionTrips.PostDistributionTrip
{
	public class PostDistributionTripCommand : IRequest<BaseResult>
	{
		public long Id { get; set; }
	}
}
