using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Products.DTOs;

namespace MarketZone.Application.Features.Products.Queries.GetProductById
{
    public class GetCompositeProductByIdQuery : IRequest<BaseResult<CompositeProductDto>>
    {
        public long Id { get; set; }
    }
}
