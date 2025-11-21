using MarketZone.Application.Helpers;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Purchases.Entities;
using MarketZone.Domain.Purchases.Enums;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.DTOs;

namespace MarketZone.Application.Features.Purchases.Commands.UpdatePurchaseInvoice
{
    public class UpdatePurchaseInvoiceCommandHandler : IRequestHandler<UpdatePurchaseInvoiceCommand, BaseResult>
    {
        private readonly IPurchaseInvoiceRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITranslator _translator;

        public UpdatePurchaseInvoiceCommandHandler(
            IPurchaseInvoiceRepository repository, 
            IUnitOfWork unitOfWork, 
            ITranslator translator)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _translator = translator;
        }

        public async Task<BaseResult> Handle(UpdatePurchaseInvoiceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _repository.GetWithDetailsByIdAsync(request.Id, cancellationToken);
                if (entity is null)
                    return new Error(ErrorCode.NotFound, _translator.GetString(TranslatorMessages.PurchaseInvoiceMessages.PurchaseInvoice_NotFound_with_id(request.Id)), nameof(request.Id));

                if (entity.Status == PurchaseInvoiceStatus.Posted)
                    return new Error(ErrorCode.AccessDenied, _translator.GetString("PurchaseInvoice_Update_NotAllowed_After_Post"), nameof(request.Id));

                entity.Update(
                    request.InvoiceNumber ?? entity.InvoiceNumber,
                    request.SupplierId ?? entity.SupplierId,
                    request.InvoiceDate ?? entity.InvoiceDate,
                    request.TotalAmount ?? entity.TotalAmount,
                    request.Discount ?? entity.Discount,
                    request.Notes ?? entity.Notes,
                    request.Currency ?? entity.Currency
                );
                
                if (request.Status.HasValue)
                {
                    entity.SetStatus(request.Status.Value);
                }
                if (request.PaymentStatus.HasValue)
                {
                    entity.SetPaymentStatus(request.PaymentStatus.Value);
                }

                if (request.Details != null)
                {
                    var existingDetails = entity.Details?.ToDictionary(d => d.Id) 
                        ?? new System.Collections.Generic.Dictionary<long, PurchaseInvoiceDetail>();

                    // تحديد IDs المرسلة في الطلب
                    var requestDetailIds = request.Details.Where(d => d.Id.HasValue).Select(d => d.Id.Value).ToHashSet();

                    // تحديد التفاصيل المحذوفة: موجودة في existingDetails لكن غير موجودة في requestDetails
                    var detailsToDelete = existingDetails.Values
                        .Where(d => !requestDetailIds.Contains(d.Id))
                        .ToList();

                    // حذف التفاصيل المحذوفة
                    // ملاحظة: PurchaseInvoice يؤثر على المخزون فقط عند Post
                    // عند Update (الفاتورة Draft)، لا حاجة لتعديل المخزون
                    foreach (var toRemove in detailsToDelete)
                    {
                        entity.Details.Remove(toRemove);
                    }

                    // تحديث أو إضافة التفاصيل المتبقية
                    foreach (var item in request.Details)
                    {
                        if (item.Id.HasValue && existingDetails.TryGetValue(item.Id.Value, out var toUpdate))
                        {
                            // تعديل سطر موجود
                            toUpdate.Update(item.ProductId, item.Quantity, item.UnitPrice, item.TotalPrice, null, item.Notes);
                        }
                        else
                        {
                            // Add new detail
                            entity.Details.Add(new PurchaseInvoiceDetail(entity.Id, item.ProductId, item.Quantity, item.UnitPrice, item.TotalPrice, item.Notes));
                        }
                    }
                }

                await _unitOfWork.SaveChangesAsync();
                return BaseResult.Ok();
            }
            catch (System.Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                var message = _translator.GetString(new TranslatorMessageDto("Error_Updating_Purchase_Invoice", new[] { ex.Message }));
                return new Error(ErrorCode.Exception, message, nameof(request.Id));
            }
        }
    }
}



