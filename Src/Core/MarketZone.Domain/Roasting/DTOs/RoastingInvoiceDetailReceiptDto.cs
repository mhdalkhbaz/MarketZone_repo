using System;

namespace MarketZone.Domain.Roasting.DTOs
{
    public class RoastingInvoiceDetailReceiptDto
    {
        public long Id { get; set; }
        public long RoastingInvoiceId { get; set; }
        public long DetailId { get; set; }
        public long ReadyProductId { get; set; }
        public string ReadyProductName { get; set; }
        public decimal QuantityKg { get; set; }
        public decimal SalePricePerKg { get; set; }
        public decimal RoastingCostPerKg { get; set; }
        public decimal CommissionPerKg { get; set; }
        public decimal NetSalePricePerKg { get; set; }
        public decimal TotalSalePrice { get; set; }
        public decimal TotalRoastingCost { get; set; }
        public decimal TotalCommission { get; set; }
        public decimal TotalNetSalePrice { get; set; }
    }
}
