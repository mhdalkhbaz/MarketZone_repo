using System;

namespace MarketZone.Domain.Roasting.DTOs
{
    public class RoastingInvoiceDetailDto
    {
        public long Id { get; set; }
        public long RoastingInvoiceId { get; set; }
        public long ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal QuantityKg { get; set; }
        public decimal RoastPricePerKg { get; set; }
        public decimal ActualQuantityAfterRoasting { get; set; }
        public decimal TotalPrice { get; set; }
        public string Notes { get; set; }
    }
}
