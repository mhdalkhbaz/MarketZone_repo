using MarketZone.Application.Helpers;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Purchases.Entities;
using MarketZone.Domain.Purchases.Enums;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MarketZone.Application.Features.Purchases.Commands.UpdatePurchaseInvoice
{
    public class UpdatePurchaseInvoiceCommandHandler(IPurchaseInvoiceRepository repository, IUnitOfWork unitOfWork, ITranslator translator) : IRequestHandler<UpdatePurchaseInvoiceCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(UpdatePurchaseInvoiceCommand request, CancellationToken cancellationToken)
        {
            var entity = await repository.GetWithDetailsByIdAsync(request.Id, cancellationToken);
            if (entity is null)
                return new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.PurchaseInvoiceMessages.PurchaseInvoice_NotFound_with_id(request.Id)), nameof(request.Id));

            if (entity.Status == PurchaseInvoiceStatus.Posted)
                return new Error(ErrorCode.AccessDenied, translator.GetString("PurchaseInvoice_Update_NotAllowed_After_Post"), nameof(request.Id));

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
                var existingDetails = entity.Details?.ToDictionary(d => d.Id) ?? new System.Collections.Generic.Dictionary<long, PurchaseInvoiceDetail>();
                foreach (var item in request.Details)
                {
                    if (item.IsDeleted && item.Id.HasValue && existingDetails.TryGetValue(item.Id.Value, out var toRemove))
                    {
                        entity.Details.Remove(toRemove);
                        continue;
                    }

                    if (item.Id.HasValue && existingDetails.TryGetValue(item.Id.Value, out var toUpdate))
                    {
                        toUpdate.Update(item.ProductId, item.Quantity, item.UnitPrice, item.TotalPrice, null, item.Notes);
                    }
                    else if (!item.IsDeleted)
                    {
                        entity.Details.Add(new PurchaseInvoiceDetail(entity.Id, item.ProductId, item.Quantity, item.UnitPrice, item.TotalPrice, item.Notes));
                    }
                }
            }

            await unitOfWork.SaveChangesAsync();
            return BaseResult.Ok();
        }
    }
}



