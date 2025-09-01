using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Logistics.Commands.DistributionTrips.CompleteTrip
{
	public class CompleteTripCommand : IRequest<BaseResult>
	{
		public long TripId { get; set; }
	}
}
