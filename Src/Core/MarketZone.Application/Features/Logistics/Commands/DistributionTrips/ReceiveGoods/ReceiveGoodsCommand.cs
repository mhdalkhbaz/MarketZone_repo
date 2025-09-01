using System.Collections.Generic;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Logistics.Commands.DistributionTrips.ReceiveGoods
{
	public class ReceiveGoodsCommand : IRequest<BaseResult>
	{
		public long TripId { get; set; }
		public List<ReceiveGoodsDetailItem> Details { get; set; } = new List<ReceiveGoodsDetailItem>();
	}

	public class ReceiveGoodsDetailItem
	{
		public long DetailId { get; set; }
		public decimal ReturnedQty { get; set; }
	}
}
