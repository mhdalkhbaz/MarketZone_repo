using System;
using System.Collections.Generic;
using MarketZone.Domain.Common;
using MarketZone.Domain.Roasting.Enums;
using MarketZone.Domain.Cash.Entities;

namespace MarketZone.Domain.Roasting.Entities
{
    public class RoastingInvoice : AuditableBaseEntity
    {
        private readonly List<RoastingInvoiceDetail> _details = new List<RoastingInvoiceDetail>();
        
        private RoastingInvoice()
        {
            Details = _details.AsReadOnly();
        }

        public RoastingInvoice(string invoiceNumber, DateTime invoiceDate, decimal totalAmount, string notes)
        {
            InvoiceNumber = invoiceNumber;
            InvoiceDate = invoiceDate;
            TotalAmount = totalAmount;
            Notes = notes;
            Status = RoastingInvoiceStatus.Draft;
            PaymentStatus = RoastingPaymentStatus.InProgress;
            Details = _details.AsReadOnly();
        }

        public string InvoiceNumber { get; private set; }
        public DateTime InvoiceDate { get; private set; }
        public decimal TotalAmount { get; private set; }
        public string Notes { get; private set; }
        public RoastingInvoiceStatus Status { get; private set; }
        public RoastingPaymentStatus PaymentStatus { get; private set; }
        public IReadOnlyCollection<RoastingInvoiceDetail> Details { get; private set; }
        public IReadOnlyCollection<Payment> Payments { get; private set; } = new List<Payment>();

        public void SetStatus(RoastingInvoiceStatus status)
        {
            Status = status;
        }

        public void SetPaymentStatus(RoastingPaymentStatus paymentStatus)
        {
            PaymentStatus = paymentStatus;
        }

        public void SetInvoiceNumber(string invoiceNumber)
        {
            InvoiceNumber = invoiceNumber;
        }

        public void AddDetail(RoastingInvoiceDetail detail)
        {
            _details.Add(detail);
        }

        public void RemoveDetail(RoastingInvoiceDetail detail)
        {
            _details.Remove(detail);
        }

        public void ClearDetails()
        {
            _details.Clear();
        }
    }
}
