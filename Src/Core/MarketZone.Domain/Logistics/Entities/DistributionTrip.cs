using MarketZone.Domain.Common;
using MarketZone.Domain.Employees.Entities;
using MarketZone.Domain.Logistics.Enums;
using MarketZone.Domain.Sales.Entities;
using System;
using System.Collections.Generic;

namespace MarketZone.Domain.Logistics.Entities
{
    public class DistributionTrip : AuditableBaseEntity
    {
        private DistributionTrip()
        {
            Details = new List<DistributionTripDetail>();
            DistributionTripSalesInvoices = new List<SalesInvoice>();
        }

        public DistributionTrip(long employeeId, long carId, long regionId, DateTime tripDate, decimal? loadQty, string notes, string tripNumber)
        {
            EmployeeId = employeeId;
            CarId = carId;
            RegionId = regionId;
            TripDate = tripDate;
            LoadQty = loadQty;
            Notes = notes ?? string.Empty;
            TripNumber = tripNumber;
            Status = DistributionTripStatus.Draft;
            Details = new List<DistributionTripDetail>();
            DistributionTripSalesInvoices = new List<SalesInvoice>();
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
        public string TripNumber { get; private set; }
        public DistributionTripStatus Status { get; private set; }

        public List<DistributionTripDetail> Details { get; private set; }
        public List<SalesInvoice> DistributionTripSalesInvoices { get; private set; }

        public void Update(long employeeId, long carId, long regionId, DateTime tripDate, decimal? loadQty, string notes)
        {
            EmployeeId = employeeId;
            CarId = carId;
            RegionId = regionId;
            TripDate = tripDate;
            LoadQty = loadQty;
            Notes = notes ?? string.Empty;
        }

        public void SetTripNumber(string tripNumber)
        {
            TripNumber = tripNumber;
        }

        public void SetStatus(DistributionTripStatus status)
        {
            Status = status;
        }

        public void AddDetail(DistributionTripDetail detail)
        {
            detail.SetTrip(this);
            Details.Add(detail);
        }

        public void AddSalesInvoice(SalesInvoice invoice)
        {
            invoice.SetDistributionTrip(this);
            DistributionTripSalesInvoices.Add(invoice);
        }
    }
}


