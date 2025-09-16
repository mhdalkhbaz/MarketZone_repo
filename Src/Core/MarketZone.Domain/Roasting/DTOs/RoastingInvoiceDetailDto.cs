using System;

namespace MarketZone.Domain.Roasting.DTOs
{
    public class RoastingInvoiceDetailDto
    {
        public long Id { get; set; }
        public long RoastingInvoiceId { get; set; }
        public long ReadyProductId { get; set; }
        public string ReadyProductName { get; set; }
        public long? RawProductId { get; set; }
        public string RawProductName { get; set; }
        public decimal QuantityKg { get; set; }
        public decimal RoastPricePerKg { get; set; }
        public decimal CommissionPerKg { get; set; }
        public decimal ActualQuantityAfterRoasting { get; set; }
        public decimal TotalPrice { get; set; }
        public string Notes { get; set; }
    }
}
