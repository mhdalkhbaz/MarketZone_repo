using System;
using System.Collections.Generic;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Logistics.Commands.DistributionTrips.UpdateDistributionTrip
{
	public class UpdateDistributionTripCommand : IRequest<BaseResult>
	{
		public long Id { get; set; }
		public long? EmployeeId { get; set; }
		public long? CarId { get; set; }
		public long? RegionId { get; set; }
		public DateTime? TripDate { get; set; }
		public decimal? LoadQty { get; set; }
		public string Notes { get; set; }
		public List<UpdateDistributionTripDetailItem> Details { get; set; }
	}

	public class UpdateDistributionTripDetailItem
	{
		public long? Id { get; set; }
		public long ProductId { get; set; }
		public decimal Qty { get; set; }
		public decimal ExpectedPrice { get; set; }
		public bool IsDeleted { get; set; }
	}
}


