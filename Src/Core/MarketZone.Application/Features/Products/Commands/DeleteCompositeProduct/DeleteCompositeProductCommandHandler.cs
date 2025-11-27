using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Products.Enums;

namespace MarketZone.Application.Features.Products.Commands.DeleteCompositeProduct
{
    public class DeleteCompositeProductCommandHandler : IRequestHandler<DeleteCompositeProductCommand, BaseResult>
    {
        private readonly ICompositeProductRepository _compositeProductRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITranslator _translator;

        public DeleteCompositeProductCommandHandler(
            ICompositeProductRepository compositeProductRepository,
            IProductRepository productRepository,
            IUnitOfWork unitOfWork,
            ITranslator translator)
        {
            _compositeProductRepository = compositeProductRepository;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
            _translator = translator;
        }

        public async Task<BaseResult> Handle(DeleteCompositeProductCommand request, CancellationToken cancellationToken)
        {
            var compositeProduct = await _compositeProductRepository.GetByIdAsync(request.Id);
            if (compositeProduct == null)
            {
                return new Error(ErrorCode.NotFound, _translator.GetString("Composite_Product_Not_Found"), nameof(request.Id));
            }

            // لا يمكن حذف المنتجات المركبة التي تم عمل Post لها
            if (compositeProduct.Status == CompositeProductStatus.Posted)
            {
                return new Error(ErrorCode.FieldDataInvalid, _translator.GetString("Cannot_Delete_Posted_Composite_Product"), nameof(request.Id));
            }

            // تحديث نوع المنتج الناتج إذا كان مركب
            var resultingProduct = await _productRepository.GetByIdAsync(compositeProduct.ResultingProductId);
            if (resultingProduct != null && resultingProduct.ProductType == ProductType.Composite)
            {
                // التحقق من عدم وجود منتجات مركبة أخرى تستخدم نفس المنتج
                // يمكن إضافة هذا التحقق لاحقاً إذا لزم الأمر
                resultingProduct.SetProductType(ProductType.Ready);
                _productRepository.Update(resultingProduct);
            }

            try
            {
                _compositeProductRepository.Delete(compositeProduct);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (System.Exception)
            {
                throw;
            }

            return new BaseResult();
        }
    }
}

