using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using System.Collections.Generic;

namespace MarketZone.Application.Features.Roasting.Commands.PostRoastingInvoice
{
    public class PostRoastingInvoiceCommand : IRequest<BaseResult<long>>
    {
        public long Id { get; set; }
        public List<PostRoastingInvoiceDetailItem> Details { get; set; } = new List<PostRoastingInvoiceDetailItem>();
    }

    public class PostRoastingInvoiceDetailItem
    {
        public long DetailId { get; set; }
        public List<PostRoastingReadyDetailItem> ReadyDetails { get; set; } = new List<PostRoastingReadyDetailItem>();
    }

    public class PostRoastingReadyDetailItem
    {
        public long RawProductId { get; set; }
        public long ReadyProductId { get; set; }
        public decimal ActualQuantityAfterRoasting { get; set; }
        public decimal CommissionPerKg { get; set; }
        public decimal NetSalePricePerKg { get; set; }
        public decimal RoastingCostPerKg { get; set; }
        public decimal SalePricePerKg { get; set; }
    }
}
