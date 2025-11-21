using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Helpers;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Sales.Entities;
using MarketZone.Domain.Sales.Enums;

namespace MarketZone.Application.Features.Sales.Commands.UpdateSalesInvoice
{
	public class UpdateSalesInvoiceCommandHandler : IRequestHandler<UpdateSalesInvoiceCommand, BaseResult>
	{
		private readonly ISalesInvoiceRepository _repository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly ITranslator _translator;
		private readonly IProductBalanceRepository _productBalanceRepository;

		public UpdateSalesInvoiceCommandHandler(
			ISalesInvoiceRepository repository,
			IUnitOfWork unitOfWork,
			ITranslator translator,
			IProductBalanceRepository productBalanceRepository)
		{
			_repository = repository;
			_unitOfWork = unitOfWork;
			_translator = translator;
			_productBalanceRepository = productBalanceRepository;
		}

		public async Task<BaseResult> Handle(UpdateSalesInvoiceCommand request, CancellationToken cancellationToken)
		{
			var entity = await _repository.GetWithDetailsByIdAsync(request.Id, cancellationToken);
			if (entity is null)
			{
				return new Error(ErrorCode.NotFound, 
					_translator.GetString(TranslatorMessages.SalesInvoiceMessages.SalesInvoice_NotFound_with_id(request.Id)), 
					nameof(request.Id));
			}

			if (entity.Status == SalesInvoiceStatus.Posted)
			{
				return new Error(ErrorCode.AccessDenied, 
					_translator.GetString("SalesInvoice_Update_NotAllowed_After_Post"), 
					nameof(request.Id));
			}

			// Update basic invoice properties
			entity.Update(
				request.InvoiceNumber ?? entity.InvoiceNumber,
				request.CustomerId ?? entity.CustomerId,
				request.InvoiceDate ?? entity.InvoiceDate,
				request.TotalAmount ?? entity.TotalAmount,
				request.Discount ?? entity.Discount,
				request.PaymentMethod ?? entity.PaymentMethod,
				request.Notes ?? entity.Notes,
				request.Currency ?? entity.Currency
			);

			// Handle details update with quantity validation
			if (request.Details != null && request.Details.Any())
			{
				var validationResult = await ValidateAndUpdateDetails(entity, request.Details, cancellationToken);
				if (!validationResult.Success)
					return validationResult;
			}

			await _unitOfWork.SaveChangesAsync();
			return BaseResult.Ok();
		}

		private async Task<BaseResult> ValidateAndUpdateDetails(
			SalesInvoice invoice,
			System.Collections.Generic.List<UpdateSalesInvoiceDetailItem> requestDetails,
			CancellationToken cancellationToken)
		{
			var existingDetails = invoice.Details?.ToDictionary(d => d.Id) 
				?? new System.Collections.Generic.Dictionary<long, SalesInvoiceDetail>();

			foreach (var item in requestDetails)
			{
				// Handle deletion
				if (item.IsDeleted && item.Id.HasValue && existingDetails.TryGetValue(item.Id.Value, out var toRemove))
				{
					invoice.Details.Remove(toRemove);
					continue;
				}

				// Skip deleted items
				if (item.IsDeleted)
					continue;

				// Validate quantity for normal sales (not distributor invoices)
				if (invoice.Type != SalesInvoiceType.Distributor)
				{
					var validationResult = await ValidateQuantityForNormalSales(invoice, item, existingDetails, cancellationToken);
					if (!validationResult.Success)
						return validationResult;
				}

				// Update existing detail
				if (item.Id.HasValue && existingDetails.TryGetValue(item.Id.Value, out var toUpdate))
				{
					toUpdate.Update(item.ProductId, item.Quantity, item.UnitPrice, item.SubTotal, item.Notes ?? string.Empty);
				}
				// Add new detail
				else
				{
					invoice.Details.Add(new SalesInvoiceDetail(
						invoice.Id,
						item.ProductId,
						item.Quantity,
						item.UnitPrice,
						item.SubTotal,
						item.Notes ?? string.Empty));
				}
			}

			return BaseResult.Ok();
		}

		private async Task<BaseResult> ValidateQuantityForNormalSales(
			SalesInvoice invoice,
			UpdateSalesInvoiceDetailItem item,
			System.Collections.Generic.Dictionary<long, SalesInvoiceDetail> existingDetails,
			CancellationToken cancellationToken)
		{
			var productBalance = await _productBalanceRepository.GetByProductIdAsync(item.ProductId, cancellationToken);
			if (productBalance == null)
			{
				return new Error(ErrorCode.NotFound,
					$"Product balance not found for product {item.ProductId}",
					nameof(UpdateSalesInvoiceCommand.Details));
			}

			// For new items, check if available quantity is sufficient
			if (!item.Id.HasValue || !existingDetails.TryGetValue(item.Id.Value, out var existingDetail))
			{
				if (productBalance.AvailableQty < item.Quantity)
				{
					return new Error(ErrorCode.FieldDataInvalid,
						$"Insufficient available quantity for product {item.ProductId}. Available: {productBalance.AvailableQty}, Requested: {item.Quantity}",
						nameof(UpdateSalesInvoiceCommand.Details));
				}
			}
			// For existing items, check if the quantity increase is within available stock
			else
			{
				var quantityDifference = item.Quantity - existingDetail.Quantity;
				if (quantityDifference > 0 && productBalance.AvailableQty < quantityDifference)
				{
					return new Error(ErrorCode.FieldDataInvalid,
						$"Insufficient available quantity for product {item.ProductId}. Available: {productBalance.AvailableQty}, Additional needed: {quantityDifference}",
						nameof(UpdateSalesInvoiceCommand.Details));
				}
			}

			return BaseResult.Ok();
		}
	}
}



