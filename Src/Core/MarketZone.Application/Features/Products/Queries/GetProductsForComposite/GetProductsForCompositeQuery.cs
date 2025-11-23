using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Products.DTOs;
using System.Collections.Generic;

namespace MarketZone.Application.Features.Products.Queries.GetProductsForComposite
{
    public class GetProductsForCompositeQuery : IRequest<BaseResult<List<ProductForCompositeDto>>>
    {
    }
}

