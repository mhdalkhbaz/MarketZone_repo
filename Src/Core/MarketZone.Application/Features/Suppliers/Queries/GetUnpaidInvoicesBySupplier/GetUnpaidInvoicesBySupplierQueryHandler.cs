using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Helpers;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.Enums;
using MarketZone.Domain.Purchases.DTOs;
using MarketZone.Domain.Purchases.Enums;

namespace MarketZone.Application.Features.Suppliers.Queries.GetUnpaidInvoicesBySupplier
{
    public class GetUnpaidInvoicesBySupplierQueryHandler(IPurchaseInvoiceRepository purchaseInvoiceRepository, IPaymentRepository paymentRepository, ITranslator translator) : IRequestHandler<GetUnpaidInvoicesBySupplierQuery, BaseResult<List<PurchaseInvoiceDto>>>
    {
        public async Task<BaseResult<List<PurchaseInvoiceDto>>> Handle(GetUnpaidInvoicesBySupplierQuery request, CancellationToken cancellationToken)
        {
            // التحقق من وجود المورد
            var supplierExists = await purchaseInvoiceRepository.SupplierExistsAsync(request.SupplierId);
            if (!supplierExists)
            {
                return new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.SupplierMessages.Supplier_NotFound_with_id(request.SupplierId)), nameof(request.SupplierId));
            }

            // الحصول على الفواتير غير المسددة للمورد
            var unpaidInvoices = await purchaseInvoiceRepository.GetUnpaidInvoicesBySupplierAsync(request.SupplierId, cancellationToken);

            return BaseResult<List<PurchaseInvoiceDto>>.Ok(unpaidInvoices);
        }
    }
}
