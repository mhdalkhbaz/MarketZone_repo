using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using System.Collections.Generic;

namespace MarketZone.Application.Features.Products.Commands.UpdateCompositeProduct
{
    public class UpdateCompositeProductCommand : IRequest<BaseResult>
    {
        public long Id { get; set; }
        public long ResultingProductId { get; set; }
        public decimal SalePrice { get; set; }
        public decimal CommissionPerKg { get; set; }
        public List<UpdateCompositeProductDetailItem> Details { get; set; } = new List<UpdateCompositeProductDetailItem>();
    }

    public class UpdateCompositeProductDetailItem
    {
        public long? Id { get; set; }
        public long ComponentProductId { get; set; }
        public decimal Quantity { get; set; }
    }
}

