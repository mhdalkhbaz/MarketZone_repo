using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Helpers;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Purchases.Enums;
using MarketZone.Application.Interfaces.Services;

namespace MarketZone.Application.Features.Purchases.Commands.PostPurchaseInvoice
{
	public class PostPurchaseInvoiceCommandHandler(IPurchaseInvoiceRepository repository, IUnitOfWork unitOfWork, ITranslator translator, IInventoryAdjustmentService inventoryAdjustment) : IRequestHandler<PostPurchaseInvoiceCommand, BaseResult>
	{
		public async Task<BaseResult> Handle(PostPurchaseInvoiceCommand request, CancellationToken cancellationToken)
		{
			var entity = await repository.GetWithDetailsByIdAsync(request.Id, cancellationToken);
			if (entity is null)
				return new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.PurchaseInvoiceMessages.PurchaseInvoice_NotFound_with_id(request.Id)), nameof(request.Id));

			entity.SetStatus(PurchaseInvoiceStatus.Posted);
			await inventoryAdjustment.AdjustOnPurchasePostedAsync(entity, cancellationToken);
			await unitOfWork.SaveChangesAsync();
			return BaseResult.Ok();
		}
	}
}


