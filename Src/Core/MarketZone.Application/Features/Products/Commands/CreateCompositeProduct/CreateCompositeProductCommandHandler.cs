using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Products.Entities;
using MarketZone.Domain.Products.Enums;

namespace MarketZone.Application.Features.Products.Commands.CreateCompositeProduct
{
    public class CreateCompositeProductCommandHandler : IRequestHandler<CreateCompositeProductCommand, BaseResult<long>>
    {
        private readonly ICompositeProductRepository _compositeProductRepository;
        private readonly IProductRepository _productRepository;
        private readonly IProductBalanceRepository _productBalanceRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITranslator _translator;

        public CreateCompositeProductCommandHandler(
            ICompositeProductRepository compositeProductRepository,
            IProductRepository productRepository,
            IProductBalanceRepository productBalanceRepository,
            IUnitOfWork unitOfWork,
            ITranslator translator)
        {
            _compositeProductRepository = compositeProductRepository;
            _productRepository = productRepository;
            _productBalanceRepository = productBalanceRepository;
            _unitOfWork = unitOfWork;
            _translator = translator;
        }

        public async Task<BaseResult<long>> Handle(CreateCompositeProductCommand request, CancellationToken cancellationToken)
        {
            // التحقق من وجود المنتج الناتج
            var resultingProduct = await _productRepository.GetByIdAsync(request.ResultingProductId);
            if (resultingProduct == null)
            {
                return new Error(ErrorCode.NotFound, _translator.GetString("Product_Not_Found"), nameof(request.ResultingProductId));
            }

            // تحديث نوع المنتج إلى مركب
            resultingProduct.SetProductType(ProductType.Composite);
            _productRepository.Update(resultingProduct);

            // إنشاء المنتج المركب
            var compositeProduct = new CompositeProduct(
                request.ResultingProductId,
                request.SalePrice,
                request.CommissionPerKg);

            // إضافة التفاصيل وحجز الكمية من AvailableQty
            foreach (var detailItem in request.Details)
            {
                // التحقق من وجود المنتج المكون
                var componentProduct = await _productRepository.GetByIdAsync(detailItem.ComponentProductId);
                if (componentProduct == null)
                {
                    return new Error(ErrorCode.NotFound, _translator.GetString("Product_Not_Found"), nameof(detailItem.ComponentProductId));
                }

                // التحقق من الكمية المتاحة
                var componentBalance = await _productBalanceRepository.GetByProductIdAsync(detailItem.ComponentProductId, cancellationToken);
                if (componentBalance == null)
                {
                    return new Error(ErrorCode.NotFound, _translator.GetString("Product_Balance_Not_Found"), nameof(detailItem.ComponentProductId));
                }

                if (componentBalance.AvailableQty < detailItem.Quantity)
                {
                    return new Error(ErrorCode.FieldDataInvalid, 
                        _translator.GetString("Insufficient_Available_Quantity"), 
                        nameof(detailItem.ComponentProductId));
                }

                // حجز الكمية من AvailableQty
                componentBalance.Adjust(0, -detailItem.Quantity);
                _productBalanceRepository.Update(componentBalance);

                var detail = new CompositeProductDetail(
                    detailItem.ComponentProductId,
                    detailItem.Quantity);

                compositeProduct.AddDetail(detail);
            }

            // التحقق من وجود تفاصيل
            if (!compositeProduct.Details.Any())
            {
                return new Error(ErrorCode.FieldDataInvalid, _translator.GetString("Composite_Product_Must_Have_Details"), nameof(request.Details));
            }

            try
            {
                await _compositeProductRepository.AddAsync(compositeProduct);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (System.Exception)
            {
                throw;
            }

            return compositeProduct.Id;
        }
    }
}

