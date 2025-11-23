using System;
using MarketZone.Domain.Cash.Enums;

namespace MarketZone.Domain.Purchases.DTOs
{
    public class PurchaseInvoiceUnpaidDto
    {
        public long Id { get; set; }
        public string InvoiceNumber { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal UnpaidAmount { get; set; }
        public Currency? Currency { get; set; }
        public DateTime InvoiceDate { get; set; }
    }
}

