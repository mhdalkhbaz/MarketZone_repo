using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Products.DTOs;

namespace MarketZone.Application.Features.Products.Queries.GetPagedListCompositeProduct
{
    public class GetPagedListCompositeProductQueryHandler(ICompositeProductRepository compositeProductRepository) : IRequestHandler<GetPagedListCompositeProductQuery, PagedResponse<CompositeProductDto>>
    {
        public async Task<PagedResponse<CompositeProductDto>> Handle(GetPagedListCompositeProductQuery request, CancellationToken cancellationToken)
        {
            return await compositeProductRepository.GetPagedListAsync(request);
        }
    }
}
