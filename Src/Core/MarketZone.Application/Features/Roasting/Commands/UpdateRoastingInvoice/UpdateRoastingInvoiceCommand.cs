using System;
using System.Collections.Generic;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Roasting.Commands.UpdateRoastingInvoice
{
    public class UpdateRoastingInvoiceCommand : IRequest<BaseResult<long>>
    {
        public long Id { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Notes { get; set; }
        public List<UpdateRoastingInvoiceDetailItem> Details { get; set; } = new List<UpdateRoastingInvoiceDetailItem>();
    }

    public class UpdateRoastingInvoiceDetailItem
    {
        public long? Id { get; set; }
        public long ReadyProductId { get; set; }
        public long? RawProductId { get; set; }
        public decimal QuantityKg { get; set; }
        public decimal RoastPricePerKg { get; set; }
        public decimal? CommissionPerKgOverride { get; set; }
        public string Notes { get; set; }
        public bool IsDeleted { get; set; }
        // لا يمكن تحديث ActualQuantityAfterRoasting - يتم إدخالها فقط عند Post
    }
}
