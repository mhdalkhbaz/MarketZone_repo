using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Products.Entities;
using MarketZone.Domain.Products.Enums;

namespace MarketZone.Application.Features.Products.Commands.UpdateCompositeProduct
{
    public class UpdateCompositeProductCommandHandler : IRequestHandler<UpdateCompositeProductCommand, BaseResult>
    {
        private readonly ICompositeProductRepository _compositeProductRepository;
        private readonly IProductRepository _productRepository;
        private readonly IProductBalanceRepository _productBalanceRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITranslator _translator;

        public UpdateCompositeProductCommandHandler(
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

        public async Task<BaseResult> Handle(UpdateCompositeProductCommand request, CancellationToken cancellationToken)
        {
            var compositeProduct = await _compositeProductRepository.GetByIdAsync(request.Id);
            if (compositeProduct == null)
            {
                return new Error(ErrorCode.NotFound, _translator.GetString("Composite_Product_Not_Found"), nameof(request.Id));
            }

            // لا يمكن التعديل إذا كان Posted
            if (compositeProduct.Status == CompositeProductStatus.Posted)
            {
                return new Error(ErrorCode.FieldDataInvalid, _translator.GetString("Cannot_Update_Posted_Composite_Product"), nameof(request.Id));
            }

            // التحقق من وجود المنتج الناتج
            var resultingProduct = await _productRepository.GetByIdAsync(request.ResultingProductId);
            if (resultingProduct == null)
            {
                return new Error(ErrorCode.NotFound, _translator.GetString("Product_Not_Found"), nameof(request.ResultingProductId));
            }

            // تحديث البيانات الأساسية
            compositeProduct.Update(request.SalePrice, request.CommissionPerKg);

            // حذف التفاصيل المحذوفة (بدون تعديل الكميات - سيتم تعديلها عند Post فقط)
            var existingDetailIds = request.Details.Where(d => d.Id.HasValue).Select(d => d.Id.Value).ToList();
            var detailsToRemove = compositeProduct.Details.Where(d => !existingDetailIds.Contains(d.Id)).ToList();
            foreach (var detail in detailsToRemove)
            {
                compositeProduct.RemoveDetail(detail);
            }

            // تحديث أو إضافة التفاصيل (بدون تعديل الكميات - سيتم تعديلها عند Post فقط)
            foreach (var detailItem in request.Details)
            {
                // التحقق من وجود المنتج المكون
                var componentProduct = await _productRepository.GetByIdAsync(detailItem.ComponentProductId);
                if (componentProduct == null)
                {
                    return new Error(ErrorCode.NotFound, _translator.GetString("Product_Not_Found"), nameof(detailItem.ComponentProductId));
                }

                if (detailItem.Id.HasValue)
                {
                    // تحديث تفصيل موجود
                    var existingDetail = compositeProduct.Details.FirstOrDefault(d => d.Id == detailItem.Id.Value);
                    if (existingDetail != null)
                    {
                        existingDetail.Update(detailItem.ComponentProductId, detailItem.Quantity);
                    }
                }
                else
                {
                    // إضافة تفصيل جديد
                    var newDetail = new CompositeProductDetail(
                        detailItem.ComponentProductId,
                        detailItem.Quantity);
                    compositeProduct.AddDetail(newDetail);
                }
            }

            // التحقق من وجود تفاصيل
            if (!compositeProduct.Details.Any())
            {
                return new Error(ErrorCode.FieldDataInvalid, _translator.GetString("Composite_Product_Must_Have_Details"), nameof(request.Details));
            }

            try
            {
                _compositeProductRepository.Update(compositeProduct);
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

