using System;
using System.Collections.Generic;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Roasting.Commands.CreateRoastingInvoice
{
    public class CreateRoastingInvoiceCommand : IRequest<BaseResult<long>>
    {
        public string InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Notes { get; set; }
        public List<CreateRoastingInvoiceDetailItem> Details { get; set; } = new List<CreateRoastingInvoiceDetailItem>();
    }

    public class CreateRoastingInvoiceDetailItem
    {
        public long? Id { get; set; }
        public long RawProductId { get; set; }
        public decimal QuantityKg { get; set; }
        public string Notes { get; set; }
        public decimal RoastingCost { get; set; }
    }
}
