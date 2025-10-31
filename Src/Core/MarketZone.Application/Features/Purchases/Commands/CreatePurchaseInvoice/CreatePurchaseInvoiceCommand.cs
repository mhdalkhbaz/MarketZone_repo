using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.Enums;
using System;
using System.Collections.Generic;

namespace MarketZone.Application.Features.Purchases.Commands.CreatePurchaseInvoice
{
    public class CreatePurchaseInvoiceCommand : IRequest<BaseResult<long>>
    {
        public string InvoiceNumber { get; set; }
        public long SupplierId { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Discount { get; set; } = 0;
        public string Notes { get; set; }
        public Currency? Currency { get; set; }

        public List<CreatePurchaseInvoiceDetailItem> Details { get; set; } = [];
    }

    public class CreatePurchaseInvoiceDetailItem
    {
        public long ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string Notes { get; set; }
    }
}



