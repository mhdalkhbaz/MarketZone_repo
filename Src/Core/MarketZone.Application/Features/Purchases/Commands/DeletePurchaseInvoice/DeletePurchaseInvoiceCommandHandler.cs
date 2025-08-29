using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Helpers;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Purchases.Commands.DeletePurchaseInvoice
{
	public class DeletePurchaseInvoiceCommandHandler(IPurchaseInvoiceRepository repository, IUnitOfWork unitOfWork, ITranslator translator) : IRequestHandler<DeletePurchaseInvoiceCommand, BaseResult>
	{
		public async Task<BaseResult> Handle(DeletePurchaseInvoiceCommand request, CancellationToken cancellationToken)
		{
			var entity = await repository.GetByIdAsync(request.Id);
			if (entity is null)
			{
				return new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.PurchaseInvoiceMessages.PurchaseInvoice_NotFound_with_id(request.Id)), nameof(request.Id));
			}
			if (entity.Status == MarketZone.Domain.Purchases.Enums.PurchaseInvoiceStatus.Posted)
			{
				return new Error(ErrorCode.AccessDenied, translator.GetString("PurchaseInvoice_Delete_NotAllowed_After_Post"), nameof(request.Id));
			}
			repository.Delete(entity);
			await unitOfWork.SaveChangesAsync();
			return BaseResult.Ok();
		}
	}
}



