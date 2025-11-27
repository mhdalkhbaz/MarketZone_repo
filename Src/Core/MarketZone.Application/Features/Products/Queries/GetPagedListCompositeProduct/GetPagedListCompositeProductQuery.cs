using MarketZone.Application.Interfaces;
using MarketZone.Application.Parameters;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Products.DTOs;

namespace MarketZone.Application.Features.Products.Queries.GetPagedListCompositeProduct
{
    public class GetPagedListCompositeProductQuery : CompositeProductFilter, IRequest<PagedResponse<CompositeProductDto>>
    {
    }
}

