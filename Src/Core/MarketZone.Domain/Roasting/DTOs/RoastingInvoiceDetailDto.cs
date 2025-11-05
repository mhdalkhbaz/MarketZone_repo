using System;
using System.Collections.Generic;

namespace MarketZone.Domain.Roasting.DTOs
{
    public class RoastingInvoiceDetailDto
    {
        public long Id { get; set; }
        public long RoastingInvoiceId { get; set; }
        public long RawProductId { get; set; }
        public string RawProductName { get; set; }
        public decimal QuantityKg { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal RoastingCost { get; set; }
        public string Notes { get; set; }
        public decimal ReceivedQuantityKg { get; set; }
        public decimal RemainingQuantity { get; set; }
        public List<RoastingInvoiceDetailReceiptDto> Receipts { get; set; } = new List<RoastingInvoiceDetailReceiptDto>();
    }
}
