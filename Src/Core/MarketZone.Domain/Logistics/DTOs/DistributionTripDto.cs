using System;
using System.Collections.Generic;
using System.Linq;
using MarketZone.Domain.Logistics.Entities;

namespace MarketZone.Domain.Logistics.DTOs
{
	public class DistributionTripDto
	{
		public DistributionTripDto()
		{
		}

		public DistributionTripDto(DistributionTrip trip)
		{
			Id = trip.Id;
			EmployeeId = trip.EmployeeId;
			CarId = trip.CarId;
			RegionId = trip.RegionId;
			TripDate = trip.TripDate;
			LoadQty = trip.LoadQty;
			Notes = trip.Notes;
			Details = trip.Details?.Select(d => new DistributionTripDetailDto(d)).ToList();
		}

		public long Id { get; set; }
		public long EmployeeId { get; set; }
		public long CarId { get; set; }
		public long RegionId { get; set; }
		public DateTime TripDate { get; set; }
		public decimal? LoadQty { get; set; }
		public string Notes { get; set; }
		public List<DistributionTripDetailDto> Details { get; set; }
	}
}


