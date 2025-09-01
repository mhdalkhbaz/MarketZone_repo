using MarketZone.Domain.Common;
using MarketZone.Domain.Products.Entities;
using System.Linq;

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
			ReturnedQty = 0; // سيتم تحديثه عند استلام البضاعة
		}

		public long TripId { get; private set; }
		public DistributionTrip Trip { get; private set; }
		public long ProductId { get; private set; }
		public Product Product { get; private set; }
		public decimal Qty { get; private set; }           // الكمية المحملة
		public decimal ExpectedPrice { get; private set; } // السعر المتوقع
		public decimal ReturnedQty { get; private set; }   // الكمية المرجعة

		// الكمية المباعة تُحسب من فواتير المبيعات المرتبطة بالرحلة
		public decimal SoldQty => Trip?.DistributionTripSalesInvoices?
			.Where(invoice => invoice.Type == MarketZone.Domain.Sales.Enums.SalesInvoiceType.Distributor)
			.SelectMany(invoice => invoice.Details)
			.Where(detail => detail.ProductId == ProductId)
			.Sum(detail => detail.Quantity) ?? 0;

		public void Update(long productId, decimal qty, decimal expectedPrice)
		{
			ProductId = productId;
			Qty = qty;
			ExpectedPrice = expectedPrice;
		}

		public void UpdateReturnedQty(decimal returnedQty)
		{
			ReturnedQty = returnedQty;
		}

		public void SetTrip(DistributionTrip trip)
		{
			Trip = trip;
			TripId = trip.Id;
		}
	}
}


