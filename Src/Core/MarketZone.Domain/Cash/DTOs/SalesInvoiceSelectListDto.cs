using System;
using MarketZone.Domain.Cash.Enums;

namespace MarketZone.Domain.Cash.DTOs
{
    public class SalesInvoiceSelectListDto
    {
        public long Id { get; set; }
        public string InvoiceNumber { get; set; }
        public decimal TotalAmount { get; set; }
        public Currency? Currency { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string Status { get; set; }
        public long? DistributionTripId { get; set; }
        public string CustomerName { get; set; }
    }
}
