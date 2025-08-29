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
		}

		public long Id { get; set; }
		public long ProductId { get; set; }
		public decimal Qty { get; set; }
		public decimal ExpectedPrice { get; set; }
	}
}


