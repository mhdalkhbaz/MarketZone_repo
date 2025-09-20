using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Logistics.Commands.DistributionTrips.CompleteTrip
{
	public class CompleteTripCommand : IRequest<BaseResult>
	{
		public long TripId { get; set; }
        public System.Collections.Generic.List<CompleteTripDetailItem> Details { get; set; } = new System.Collections.Generic.List<CompleteTripDetailItem>();
	}

    public class CompleteTripDetailItem
    {
        public long DetailId { get; set; }
        public decimal ReturnedQty { get; set; }
    }
}
