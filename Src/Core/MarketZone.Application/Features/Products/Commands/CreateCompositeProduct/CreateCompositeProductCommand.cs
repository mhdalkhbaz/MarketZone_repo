using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using System.Collections.Generic;

namespace MarketZone.Application.Features.Products.Commands.CreateCompositeProduct
{
    public class CreateCompositeProductCommand : IRequest<BaseResult<long>>
    {
        public long ResultingProductId { get; set; }
        public decimal SalePrice { get; set; }
        public decimal CommissionPerKg { get; set; }
        public List<CreateCompositeProductDetailItem> Details { get; set; } = new List<CreateCompositeProductDetailItem>();
    }

    public class CreateCompositeProductDetailItem
    {
        public long ComponentProductId { get; set; }
        public decimal Quantity { get; set; }
    }
}

