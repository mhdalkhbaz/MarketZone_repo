using MarketZone.Application.Interfaces;
using MarketZone.Application.Parameters;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Products.DTOs;

namespace MarketZone.Application.Features.Products.Queries.GetPagedListProduct
{
    public class GetPagedListProductQuery : PaginationRequestParameter, IRequest<PagedResponse<ProductDto>>
    {
        public string Name { get; set; }
    }
}
