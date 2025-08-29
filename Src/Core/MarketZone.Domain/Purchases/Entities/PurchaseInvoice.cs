using MarketZone.Domain.Common;
using MarketZone.Domain.Purchases.Enums;
using MarketZone.Domain.Suppliers.Entities;
using System;
using System.Collections.Generic;

namespace MarketZone.Domain.Purchases.Entities
{
    public class PurchaseInvoice : AuditableBaseEntity
    {
#pragma warning disable
        private PurchaseInvoice()
        {
        }
#pragma warning restore
        public PurchaseInvoice(string invoiceNumber, long supplierId, DateTime? invoiceDate, decimal totalAmount, decimal discount, string notes)
        {
            InvoiceNumber = invoiceNumber;
            SupplierId = supplierId;
            InvoiceDate = invoiceDate ?? DateTime.UtcNow;
            TotalAmount = totalAmount;
            Discount = discount;
            Notes = notes;
            Status = PurchaseInvoiceStatus.Draft;
            PaymentStatus = PurchasePaymentStatus.InProgress;
        }

        public PurchaseInvoice(string invoiceNumber, long supplierId, DateTime? invoiceDate, decimal totalAmount, decimal discount, string notes, PurchaseInvoiceStatus status, PurchasePaymentStatus paymentStatus)
        {
            InvoiceNumber = invoiceNumber;
            SupplierId = supplierId;
            InvoiceDate = invoiceDate ?? DateTime.UtcNow;
            TotalAmount = totalAmount;
            Discount = discount;
            Notes = notes;
            Status = status;
            PaymentStatus = paymentStatus;
        }

        public string InvoiceNumber { get; private set; }
        public long SupplierId { get; private set; }
        public Supplier Supplier { get; private set; }
        public DateTime InvoiceDate { get; private set; }
        public decimal TotalAmount { get; private set; }
        public decimal Discount { get; private set; }
        public string Notes { get; private set; }
        public PurchaseInvoiceStatus Status { get; private set; }
        public PurchasePaymentStatus? PaymentStatus { get; private set; }

        public void SetStatus(PurchaseInvoiceStatus status)
        {
            Status = status;
        }

        public void SetPaymentStatus(PurchasePaymentStatus paymentStatus)
        {
            PaymentStatus = paymentStatus;
        }

        public void SetInvoiceNumber(string invoiceNumber)
        {
            InvoiceNumber = invoiceNumber;
        }

        public List<PurchaseInvoiceDetail> Details { get; private set; }

        public void Update(string invoiceNumber, long supplierId, DateTime invoiceDate, decimal totalAmount, decimal discount, string notes)
        {
            InvoiceNumber = invoiceNumber;
            SupplierId = supplierId;
            InvoiceDate = invoiceDate;
            TotalAmount = totalAmount;
            Discount = discount;
            Notes = notes;
        }
    }
}



