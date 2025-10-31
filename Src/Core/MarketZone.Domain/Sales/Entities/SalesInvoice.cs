using MarketZone.Domain.Common;
using MarketZone.Domain.Customers.Entities;
using MarketZone.Domain.Logistics.Entities;
using MarketZone.Domain.Sales.Enums;
using MarketZone.Domain.Cash.Enums;
using System;
using System.Collections.Generic;

namespace MarketZone.Domain.Sales.Entities
{
    public class SalesInvoice : AuditableBaseEntity
    {
        private SalesInvoice()
        {
            Details = new List<SalesInvoiceDetail>();
        }

        public SalesInvoice(string invoiceNumber, long customerId, DateTime invoiceDate, decimal totalAmount, decimal discount, string paymentMethod, string notes, Currency? currency = null)
        {
            InvoiceNumber = invoiceNumber;
            CustomerId = customerId;
            InvoiceDate = invoiceDate;
            TotalAmount = totalAmount;
            Discount = discount;
            PaymentMethod = paymentMethod;
            Notes = notes ?? string.Empty;
            Currency = currency;
            Status = SalesInvoiceStatus.Draft;
            Type = SalesInvoiceType.Regular;
            Details = new List<SalesInvoiceDetail>();
        }

        public string InvoiceNumber { get; private set; }
        public long CustomerId { get; private set; }
        public Customer Customer { get; private set; }
        public DateTime InvoiceDate { get; private set; }
        public decimal TotalAmount { get; private set; }
        public decimal Discount { get; private set; }
        public string PaymentMethod { get; private set; }
        public string Notes { get; private set; }
        public Currency? Currency { get; private set; }
        public SalesInvoiceStatus Status { get; private set; }
        public SalesInvoiceType Type { get; private set; }
        
        // ربط برحلة التوزيع (اختياري)
        public long? DistributionTripId { get; private set; }
        public DistributionTrip DistributionTrip { get; private set; }

        public List<SalesInvoiceDetail> Details { get; private set; }

        public void Update(string invoiceNumber, long customerId, DateTime invoiceDate, decimal totalAmount, decimal discount, string paymentMethod, string notes, Currency? currency = null)
        {
            InvoiceNumber = invoiceNumber;
            CustomerId = customerId;
            InvoiceDate = invoiceDate;
            TotalAmount = totalAmount;
            Discount = discount;
            PaymentMethod = paymentMethod;
            Notes = notes ?? string.Empty;
            Currency = currency;
        }

        public void SetStatus(SalesInvoiceStatus status)
        {
            Status = status;
        }

        public void SetType(SalesInvoiceType type)
        {
            Type = type;
        }

        public void SetDistributionTrip(DistributionTrip trip)
        {
            DistributionTrip = trip;
            DistributionTripId = trip.Id;
        }

        public void AddDetail(SalesInvoiceDetail detail)
        {
            detail.SetInvoice(this);
            Details.Add(detail);
        }
    }
}



