using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MarketZone.Application.Helpers;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Products.Commands.UpdateProduct
{
    public class UpdateProductCommandHandler(IProductRepository productRepository, IUnitOfWork unitOfWork, ITranslator translator, IMapper mapper) : IRequestHandler<UpdateProductCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await productRepository.GetByIdAsync(request.Id);

            if (product is null)
            {
                return new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.ProductMessages.Product_NotFound_with_id(request.Id)), nameof(request.Id));
            }

            mapper.Map(request, product);

            // Update commission if provided
            product.SetCommissionPerKg(request.CommissionPerKg);
            
            // Update product type if provided
            if (request.ProductType.HasValue)
            {
                product.SetProductType(request.ProductType.Value);
            }
            
            await unitOfWork.SaveChangesAsync();

            return BaseResult.Ok();
        }
    }
}
