using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MarketZone.Application.Helpers;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Products.DTOs;

namespace MarketZone.Application.Features.Products.Queries.GetProductById
{
    public class GetCompasetProductByIdQueryHandler(ICompositeProductRepository productRepository, IProductBalanceRepository productBalanceRepository, ITranslator translator, IMapper mapper) : IRequestHandler<GetCompositeProductByIdQuery, BaseResult<CompositeProductDto>>
    {
        public async Task<BaseResult<CompositeProductDto>> Handle(GetCompositeProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await productRepository.GetByIdsAsync(request.Id);

            if (product is null)
            {
                return new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.ProductMessages.Product_NotFound_with_id(request.Id)), nameof(request.Id));
            }

            var productDto = mapper.Map<CompositeProductDto>(product);

            return productDto;
        }
    }
}
