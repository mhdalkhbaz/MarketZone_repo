using MarketZone.Domain.Common;
using MarketZone.Domain.Products.Entities;

namespace MarketZone.Domain.Logistics.Entities
{
	public class DistributionTripDetail : AuditableBaseEntity
	{
		private DistributionTripDetail()
		{
		}

		public DistributionTripDetail(long tripId, long productId, decimal qty, decimal expectedPrice)
		{
			TripId = tripId;
			ProductId = productId;
			Qty = qty;
			ExpectedPrice = expectedPrice;
		}

		public long TripId { get; private set; }
		public DistributionTrip Trip { get; private set; }
		public long ProductId { get; private set; }
		public Product Product { get; private set; }
		public decimal Qty { get; private set; }
		public decimal ExpectedPrice { get; private set; }

		public void Update(long productId, decimal qty, decimal expectedPrice)
		{
			ProductId = productId;
			Qty = qty;
			ExpectedPrice = expectedPrice;
		}
	}
}


