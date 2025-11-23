using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Products.Commands.PostCompositeProduct
{
    public class PostCompositeProductCommand : IRequest<BaseResult>
    {
        public long Id { get; set; }
    }
}

