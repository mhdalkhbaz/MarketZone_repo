using System;
using System.Collections.Generic;
using MarketZone.Domain.Roasting.Enums;
using MarketZone.Domain.Cash.DTOs;

namespace MarketZone.Domain.Roasting.DTOs
{
    public class RoastingInvoiceDto
    {
        public long Id { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Notes { get; set; }
        public long? EmployeeId { get; set; }
        public RoastingInvoiceStatus Status { get; set; }
        public RoastingPaymentStatus PaymentStatus { get; set; }
        public DateTime Created { get; set; }
        public DateTime? LastModified { get; set; }
        public List<RoastingInvoiceDetailDto> Details { get; set; } = new List<RoastingInvoiceDetailDto>();
        public List<PaymentDto> Payments { get; set; } = new List<PaymentDto>();
    }
}
