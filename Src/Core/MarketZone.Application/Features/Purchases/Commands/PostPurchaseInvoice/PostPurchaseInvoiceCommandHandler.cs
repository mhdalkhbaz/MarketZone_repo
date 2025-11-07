using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Helpers;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Purchases.Enums;
using MarketZone.Application.Interfaces.Services;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Domain.Cash.Enums;

namespace MarketZone.Application.Features.Purchases.Commands.PostPurchaseInvoice
{
	public class PostPurchaseInvoiceCommandHandler(IPurchaseInvoiceRepository repository, ISupplierRepository supplierRepository, IUnitOfWork unitOfWork, ITranslator translator, IInventoryAdjustmentService inventoryAdjustment, IExchangeRateRepository exchangeRateRepository) : IRequestHandler<PostPurchaseInvoiceCommand, BaseResult>
	{
		public async Task<BaseResult> Handle(PostPurchaseInvoiceCommand request, CancellationToken cancellationToken)
		{
			var entity = await repository.GetWithDetailsByIdAsync(request.Id, cancellationToken);
			if (entity is null)
				return new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.PurchaseInvoiceMessages.PurchaseInvoice_NotFound_with_id(request.Id)), nameof(request.Id));

			// Ensure currency: derive from supplier if missing
			if (!entity.Currency.HasValue)
			{
				var supplier = await supplierRepository.GetByIdAsync(entity.SupplierId);
				if (supplier?.Currency != null)
				{
					// Update invoice currency from supplier default
					entity.Update(entity.InvoiceNumber, entity.SupplierId, entity.InvoiceDate, entity.TotalAmount, entity.Discount, entity.Notes, supplier.Currency);
				}
				else
				{
					return new Error(ErrorCode.FieldDataInvalid, translator.GetString("PurchaseInvoice_Currency_Required"), nameof(entity.Currency));
				}
			}

			// If SYP currency, ensure there is an active exchange rate before proceeding
			if (entity.Currency.Value == Currency.SY)
			{
				var latestRate = await exchangeRateRepository.GetLatestActiveRateAsync(cancellationToken);
				if (latestRate == null || latestRate.Rate <= 0)
					return new Error(ErrorCode.FieldDataInvalid, translator.GetString("Active_Exchange_Rate_Required"), nameof(entity.Currency));
			}

			entity.SetStatus(PurchaseInvoiceStatus.Posted);
			await inventoryAdjustment.AdjustOnPurchasePostedAsync(entity, cancellationToken);
			await unitOfWork.SaveChangesAsync();
			return BaseResult.Ok();
		}
	}
}


