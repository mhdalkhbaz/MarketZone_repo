using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Products.Entities;
using MarketZone.Domain.Products.Enums;
using MarketZone.Domain.Inventory.Entities;

namespace MarketZone.Application.Features.Products.Commands.PostCompositeProduct
{
    public class PostCompositeProductCommandHandler : IRequestHandler<PostCompositeProductCommand, BaseResult>
    {
        private readonly ICompositeProductRepository _compositeProductRepository;
        private readonly IProductRepository _productRepository;
        private readonly IProductBalanceRepository _productBalanceRepository;
        private readonly IInventoryHistoryRepository _inventoryHistoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITranslator _translator;

        public PostCompositeProductCommandHandler(
            ICompositeProductRepository compositeProductRepository,
            IProductRepository productRepository,
            IProductBalanceRepository productBalanceRepository,
            IInventoryHistoryRepository inventoryHistoryRepository,
            IUnitOfWork unitOfWork,
            ITranslator translator)
        {
            _compositeProductRepository = compositeProductRepository;
            _productRepository = productRepository;
            _productBalanceRepository = productBalanceRepository;
            _inventoryHistoryRepository = inventoryHistoryRepository;
            _unitOfWork = unitOfWork;
            _translator = translator;
        }

        public async Task<BaseResult> Handle(PostCompositeProductCommand request, CancellationToken cancellationToken)
        {
            var compositeProduct = await _compositeProductRepository.GetWithDetailsByIdAsync(request.Id, cancellationToken);
            if (compositeProduct == null)
            {
                return new Error(ErrorCode.NotFound, _translator.GetString("Composite_Product_Not_Found"), nameof(request.Id));
            }

            if (compositeProduct.Status == CompositeProductStatus.Posted)
            {
                return new Error(ErrorCode.FieldDataInvalid, _translator.GetString("Composite_Product_Already_Posted"), nameof(request.Id));
            }

            // التحقق من أن المنتج في حالة Draft
            if (compositeProduct.Status != CompositeProductStatus.Draft)
            {
                return new Error(ErrorCode.FieldDataInvalid, _translator.GetString("Composite_Product_Must_Be_In_Draft_Status"), nameof(request.Id));
            }

            // حساب الكمية الناتجة: مجموع الكميات المدخلة لكل المكونات
            var resultingQuantity = compositeProduct.Details.Any()
                ? compositeProduct.Details.Sum(d => d.Quantity)
                : 0;

            if (resultingQuantity <= 0)
            {
                return new Error(ErrorCode.FieldDataInvalid, _translator.GetString("Invalid_Resulting_Quantity"), nameof(request.Id));
            }

            // استهلاك الكمية من Qty و AvailableQty للمنتجات المكونة
            foreach (var detail in compositeProduct.Details)
            {
                var componentBalance = await _productBalanceRepository.GetByProductIdAsync(detail.ComponentProductId, cancellationToken);
                if (componentBalance == null)
                {
                    return new Error(ErrorCode.NotFound, _translator.GetString("Product_Balance_Not_Found"), nameof(detail.ComponentProductId));
                }

                // التحقق من الكمية المتاحة
                if (componentBalance.AvailableQty < detail.Quantity)
                {
                    return new Error(ErrorCode.FieldDataInvalid,
                        _translator.GetString("Insufficient_Available_Quantity"),
                        nameof(detail.ComponentProductId));
                }

                // التحقق من الكمية الأساسية
                if (componentBalance.Qty < detail.Quantity)
                {
                    return new Error(ErrorCode.FieldDataInvalid,
                        _translator.GetString("Insufficient_Quantity"),
                        nameof(detail.ComponentProductId));
                }

                // استهلاك الكمية من Qty و AvailableQty
                componentBalance.Adjust(-detail.Quantity, -detail.Quantity);
                _productBalanceRepository.Update(componentBalance);

                // إنشاء سجل في InventoryHistory
                var inventoryHistory = new InventoryHistory(
                    detail.ComponentProductId,
                    "CompositeProduct",
                    compositeProduct.Id,
                    -detail.Quantity,
                    System.DateTime.UtcNow,
                    $"Composite product consumption - Product: {compositeProduct.ResultingProductId}");
                await _inventoryHistoryRepository.AddAsync(inventoryHistory);
            }

            // حساب القيمة للمنتج المركب (من المكونات)
            decimal totalValue = 0;
            foreach (var detail in compositeProduct.Details)
            {
                var componentBalance = await _productBalanceRepository.GetByProductIdAsync(detail.ComponentProductId, cancellationToken);
                if (componentBalance != null)
                {
                    // استخدام AverageCost للمنتج المكون
                    var componentValue = componentBalance.AverageCost * detail.Quantity;
                    totalValue += componentValue;
                }
            }

            // إضافة المنتج المركب إلى المخزون
            var resultingBalance = await _productBalanceRepository.GetByProductIdAsync(compositeProduct.ResultingProductId, cancellationToken);
            if (resultingBalance == null)
            {
                // إنشاء ProductBalance جديد
                resultingBalance = new ProductBalance(
                    compositeProduct.ResultingProductId,
                    resultingQuantity,
                    resultingQuantity,
                    totalValue,
                    compositeProduct.SalePrice);
                await _productBalanceRepository.AddAsync(resultingBalance);
            }
            else
            {
                // تحديث ProductBalance الموجود
                resultingBalance.AdjustWithValue(resultingQuantity, resultingQuantity, totalValue);
                if (compositeProduct.SalePrice > 0)
                {
                    resultingBalance.SetSalePrice(compositeProduct.SalePrice);
                }
                _productBalanceRepository.Update(resultingBalance);
            }

            // تحديث CommissionPerKg للمنتج الناتج
            var resultingProduct = await _productRepository.GetByIdAsync(compositeProduct.ResultingProductId);
            if (resultingProduct != null && compositeProduct.CommissionPerKg > 0)
            {
                resultingProduct.SetCommissionPerKg(compositeProduct.CommissionPerKg);
                _productRepository.Update(resultingProduct);
            }

            // إنشاء سجل في InventoryHistory للمنتج المركب
            var resultingInventoryHistory = new InventoryHistory(
                compositeProduct.ResultingProductId,
                "CompositeProduct",
                compositeProduct.Id,
                resultingQuantity,
                System.DateTime.UtcNow,
                $"Composite product created - Quantity: {resultingQuantity}");
            await _inventoryHistoryRepository.AddAsync(resultingInventoryHistory);

            // تحديث الحالة إلى Posted
            compositeProduct.SetStatus(CompositeProductStatus.Posted);
            _compositeProductRepository.Update(compositeProduct);

            await _unitOfWork.SaveChangesAsync();
            return BaseResult.Ok();
        }
    }
}

