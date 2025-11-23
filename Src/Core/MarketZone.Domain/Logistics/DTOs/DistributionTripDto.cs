using System;
using System.Collections.Generic;
using System.Linq;
using MarketZone.Domain.Logistics.Entities;
using MarketZone.Domain.Logistics.Enums;

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
			EmployeeName = trip.Employee != null ? $"{trip.Employee.FirstName} {trip.Employee.LastName}" : string.Empty;
			CarId = trip.CarId;
			CarName = trip.Car?.Name ?? string.Empty;
			RegionId = trip.RegionId;
			RegionName = trip.Region?.Name ?? string.Empty;
			TripDate = trip.TripDate;
			LoadQty = trip.LoadQty;
			Notes = trip.Notes;
			TripNumber = trip.TripNumber;
			Status = trip.Status;
			Details = trip.Details?.Select(d => new DistributionTripDetailDto(d)).ToList() ?? new List<DistributionTripDetailDto>();
		}

		public long Id { get; set; }
		public long EmployeeId { get; set; }
		public string EmployeeName { get; set; }
		public long CarId { get; set; }
		public string CarName { get; set; }
		public long RegionId { get; set; }
		public string RegionName { get; set; }
		public DateTime TripDate { get; set; }
		public decimal? LoadQty { get; set; }
		public string Notes { get; set; }
		public string TripNumber { get; set; }
		public DistributionTripStatus Status { get; set; }
		public List<DistributionTripDetailDto> Details { get; set; } = new List<DistributionTripDetailDto>();
	}
}


