using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Products.DTOs;

namespace MarketZone.Application.Features.Products.Queries.GetProductsForComposite
{
    public class GetProductsForCompositeQueryHandler : IRequestHandler<GetProductsForCompositeQuery, BaseResult<List<ProductForCompositeDto>>>
    {
        private readonly IProductRepository _productRepository;

        public GetProductsForCompositeQueryHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<BaseResult<List<ProductForCompositeDto>>> Handle(GetProductsForCompositeQuery request, CancellationToken cancellationToken)
        {
            var products = await _productRepository.GetProductsForCompositeAsync(cancellationToken);
            return products;
        }
    }
}

