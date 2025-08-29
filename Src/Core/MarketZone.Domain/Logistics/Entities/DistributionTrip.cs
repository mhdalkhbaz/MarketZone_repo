using MarketZone.Domain.Common;
using MarketZone.Domain.Employees.Entities;
using System;
using System.Collections.Generic;

namespace MarketZone.Domain.Logistics.Entities
{
    public class DistributionTrip : AuditableBaseEntity
    {
        private DistributionTrip()
        {
        }

        public DistributionTrip(long employeeId, long carId, long regionId, DateTime tripDate, decimal? loadQty, string notes)
        {
            EmployeeId = employeeId;
            CarId = carId;
            RegionId = regionId;
            TripDate = tripDate;
            LoadQty = loadQty;
            Notes = notes;
        }

        public long EmployeeId { get; private set; }
        public Employee Employee { get; private set; }
        public long CarId { get; private set; }
        public Car Car { get; private set; }
        public long RegionId { get; private set; }
        public Region Region { get; private set; }
        public DateTime TripDate { get; private set; }
        public decimal? LoadQty { get; private set; }
        public string Notes { get; private set; }

        public List<DistributionTripDetail> Details { get; private set; }

        public void Update(long employeeId, long carId, long regionId, DateTime tripDate, decimal? loadQty, string notes)
        {
            EmployeeId = employeeId;
            CarId = carId;
            RegionId = regionId;
            TripDate = tripDate;
            LoadQty = loadQty;
            Notes = notes;
        }
    }
}


