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
        public decimal ActualQuantityAfterRoasting { get; set; } // الكمية الفعلية بعد التحميص
    }
}
