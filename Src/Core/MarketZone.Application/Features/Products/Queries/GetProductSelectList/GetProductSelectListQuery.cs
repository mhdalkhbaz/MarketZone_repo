using MarketZone.Application.DTOs;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using System.Collections.Generic;

namespace MarketZone.Application.Features.Products.Queries.GetProductSelectList
{
    public class GetProductSelectListQuery : IRequest<BaseResult<List<ProductSelectListDto>>>
    {
        public long? DistributionTripId { get; set; } // إذا كان موجود، تجلب المنتجات المتبقية في الرحلة
    }
}
