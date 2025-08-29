using MarketZone.Domain.Common;
using MarketZone.Domain.Customers.Entities;
using System;
using System.Collections.Generic;
using MarketZone.Domain.Sales.Enums;

namespace MarketZone.Domain.Sales.Entities
{
    public class SalesInvoice : AuditableBaseEntity
    {
#pragma warning disable
        private SalesInvoice()
        {
        }
#pragma warning restore
        public SalesInvoice(string invoiceNumber, long? customerId, DateTime? invoiceDate, decimal totalAmount, decimal discount, string paymentMethod, string notes)
        {
            InvoiceNumber = invoiceNumber;
            CustomerId = customerId;
            InvoiceDate = invoiceDate ?? DateTime.UtcNow;
            TotalAmount = totalAmount;
            Discount = discount;
            PaymentMethod = paymentMethod;
            Notes = notes;
        }

        public string InvoiceNumber { get; private set; }
        public long? CustomerId { get; private set; }
        public Customer Customer { get; private set; }
        public DateTime InvoiceDate { get; private set; }
        public decimal TotalAmount { get; private set; }
        public decimal Discount { get; private set; }
        public string PaymentMethod { get; private set; }
        public string Notes { get; private set; }
        public SalesInvoiceStatus Status { get; private set; }

        public List<SalesInvoiceDetail> Details { get; private set; }

        public void Update(string invoiceNumber, long? customerId, DateTime invoiceDate, decimal totalAmount, decimal discount, string paymentMethod, string notes)
        {
            InvoiceNumber = invoiceNumber;
            CustomerId = customerId;
            InvoiceDate = invoiceDate;
            TotalAmount = totalAmount;
            Discount = discount;
            PaymentMethod = paymentMethod;
            Notes = notes;
        }

        public void SetStatus(SalesInvoiceStatus status)
        {
            Status = status;
        }
    }
}



