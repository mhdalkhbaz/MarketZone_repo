using MarketZone.Domain.Purchases.Entities;
using MarketZone.Domain.Purchases.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MarketZone.Domain.Purchases.DTOs
{
    public class PurchaseInvoiceDto
    {
#pragma warning disable
        public PurchaseInvoiceDto()
        {
        }
#pragma warning restore
        public PurchaseInvoiceDto(PurchaseInvoice invoice)
        {
            Id = invoice.Id;
            InvoiceNumber = invoice.InvoiceNumber;
            SupplierId = invoice.SupplierId;
            InvoiceDate = invoice.InvoiceDate;
            TotalAmount = invoice.TotalAmount;
            Discount = invoice.Discount;
            Notes = invoice.Notes;
            Status = invoice.Status;
            PaymentStatus = invoice.PaymentStatus.Value;
            Details = invoice.Details?.Select(d => new PurchaseInvoiceDetailDto(d)).ToList();
        }

        public long Id { get; set; }
        public string InvoiceNumber { get; set; }
        public long SupplierId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Discount { get; set; }
        public string Notes { get; set; }
        public PurchaseInvoiceStatus Status { get; set; }
        public PurchasePaymentStatus? PaymentStatus { get; set; }
        public List<PurchaseInvoiceDetailDto> Details { get; set; }
    }
}



