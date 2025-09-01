using MarketZone.Domain.Logistics.Entities;

namespace MarketZone.Domain.Logistics.DTOs
{
	public class DistributionTripDetailDto
	{
		public DistributionTripDetailDto()
		{
		}

		public DistributionTripDetailDto(DistributionTripDetail detail)
		{
			Id = detail.Id;
			ProductId = detail.ProductId;
			Qty = detail.Qty;
			ExpectedPrice = detail.ExpectedPrice;
			ReturnedQty = detail.ReturnedQty;
			SoldQty = detail.SoldQty; // محسوب من فواتير المبيعات
		}

		public long Id { get; set; }
		public long ProductId { get; set; }
		public decimal Qty { get; set; }           // الكمية المحملة
		public decimal ExpectedPrice { get; set; } // السعر المتوقع
		public decimal ReturnedQty { get; set; }   // الكمية المرجعة
		public decimal SoldQty { get; set; }       // الكمية المباعة (محسوبة)
	}
}


