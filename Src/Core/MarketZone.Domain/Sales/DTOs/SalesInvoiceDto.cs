using MarketZone.Domain.Sales.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MarketZone.Domain.Sales.DTOs
{
    public class SalesInvoiceDto
    {
#pragma warning disable
        public SalesInvoiceDto()
        {
        }
#pragma warning restore
        public SalesInvoiceDto(SalesInvoice invoice)
        {
            Id = invoice.Id;
            InvoiceNumber = invoice.InvoiceNumber;
            CustomerId = invoice.CustomerId;
            InvoiceDate = invoice.InvoiceDate;
            TotalAmount = invoice.TotalAmount;
            Discount = invoice.Discount;
            PaymentMethod = invoice.PaymentMethod;
            Notes = invoice.Notes;
            Details = invoice.Details?.Select(d => new SalesInvoiceDetailDto(d)).ToList();
        }

        public long Id { get; set; }
        public string InvoiceNumber { get; set; }
        public long? CustomerId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Discount { get; set; }
        public string PaymentMethod { get; set; }
        public string Notes { get; set; }
        public List<SalesInvoiceDetailDto> Details { get; set; }
    }
}



