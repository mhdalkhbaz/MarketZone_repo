using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Logistics.Commands.DistributionTrips.DeleteDistributionTrip
{
	public class DeleteDistributionTripCommand : IRequest<BaseResult>
	{
		public long TripId { get; set; }

		public DeleteDistributionTripCommand(long tripId)
		{
			TripId = tripId;
		}
	}
}

