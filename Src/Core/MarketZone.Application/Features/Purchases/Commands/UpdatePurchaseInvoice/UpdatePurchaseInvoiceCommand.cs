using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using System;
using System.Collections.Generic;
using MarketZone.Domain.Purchases.Enums;

namespace MarketZone.Application.Features.Purchases.Commands.UpdatePurchaseInvoice
{
    public class UpdatePurchaseInvoiceCommand : IRequest<BaseResult>
    {
        public long Id { get; set; }
        public string InvoiceNumber { get; set; }
        public long? SupplierId { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? Discount { get; set; }
        public string Notes { get; set; }
        public PurchaseInvoiceStatus? Status { get; set; }
        public PurchasePaymentStatus? PaymentStatus { get; set; }
        public List<UpdatePurchaseInvoiceDetailItem> Details { get; set; }

    }

    public class UpdatePurchaseInvoiceDetailItem
    {
        public long? Id { get; set; }
        public long ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string Notes { get; set; }
        public bool IsDeleted { get; set; }
    }
}



