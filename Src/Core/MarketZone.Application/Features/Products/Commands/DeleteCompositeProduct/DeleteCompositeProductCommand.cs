using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Products.Commands.DeleteCompositeProduct
{
    public class DeleteCompositeProductCommand : IRequest<BaseResult>
    {
        public long Id { get; set; }
    }
}

