using System;
using System.Collections.Generic;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Logistics.Commands.DistributionTrips.CreateDistributionTrip
{
	public class CreateDistributionTripCommand : IRequest<BaseResult<long>>
	{
		public long EmployeeId { get; set; }
		public long CarId { get; set; }
		public long RegionId { get; set; }
		public DateTime TripDate { get; set; }
		public decimal? LoadQty { get; set; }
		public string Notes { get; set; }
		public List<CreateDistributionTripDetailItem> Details { get; set; } = [];
	}

	public class CreateDistributionTripDetailItem
	{
		public long ProductId { get; set; }
		public decimal Qty { get; set; }
		public decimal? ExpectedPrice { get; set; }
	}
}


