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
    public class GetProductByIdQueryHandler(IProductRepository productRepository, IProductBalanceRepository productBalanceRepository, ITranslator translator, IMapper mapper) : IRequestHandler<GetProductByIdQuery, BaseResult<ProductDto>>
    {
        public async Task<BaseResult<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await productRepository.GetByIdAsync(request.Id);

            if (product is null)
            {
                return new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.ProductMessages.Product_NotFound_with_id(request.Id)), nameof(request.Id));
            }

            var productDto = mapper.Map<ProductDto>(product);
            
            // جلب الكمية من ProductBalances
            var productBalance = await productBalanceRepository.GetByProductIdAsync(request.Id, cancellationToken);
                productDto.Qty = productBalance?.Qty  ?? 0;
                productDto.AvailableQty = productBalance?.AvailableQty ?? 0 ;
                productDto.AverageCost = productBalance?.AverageCost ?? 0;
                productDto.SalePrice = productBalance?.SalePrice ?? 0;
          

            return productDto;
        }
    }
}
