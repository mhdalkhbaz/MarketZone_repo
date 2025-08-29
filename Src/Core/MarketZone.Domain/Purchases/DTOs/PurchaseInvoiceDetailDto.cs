using MarketZone.Domain.Purchases.Entities;
using System;

namespace MarketZone.Domain.Purchases.DTOs
{
    public class PurchaseInvoiceDetailDto
    {
#pragma warning disable
        public PurchaseInvoiceDetailDto()
        {
        }
#pragma warning restore
        public PurchaseInvoiceDetailDto(PurchaseInvoiceDetail detail)
        {
            Id = detail.Id;
            ProductId = detail.ProductId;
            Quantity = detail.Quantity;
            UnitPrice = detail.UnitPrice;
            TotalPrice = detail.TotalPrice;
            Notes = detail.Notes;
        }

        public long Id { get; set; }
        public long ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string BatchNumber { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string Notes { get; set; }
    }
}



