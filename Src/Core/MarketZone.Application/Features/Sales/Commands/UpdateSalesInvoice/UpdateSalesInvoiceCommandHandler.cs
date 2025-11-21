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

			// تحديد IDs المرسلة في الطلب
			var requestDetailIds = requestDetails.Where(d => d.Id.HasValue).Select(d => d.Id.Value).ToHashSet();

			// تحديد التفاصيل المحذوفة: موجودة في existingDetails لكن غير موجودة في requestDetails
			var detailsToDelete = existingDetails.Values
				.Where(d => !requestDetailIds.Contains(d.Id))
				.ToList();

		// أولاً: التحقق من الكميات المتاحة للمبيعات العادية
		// نحسب الكميات التي سيتم إرجاعها من الحذف والتعديلات لأخذها في الاعتبار عند التحقق
		var quantitiesToReturn = new System.Collections.Generic.Dictionary<long, decimal>();
		
		if (invoice.Type != SalesInvoiceType.Distributor)
		{
			// حساب الكميات التي سيتم إرجاعها من الحذف
			foreach (var toRemove in detailsToDelete)
			{
				if (!quantitiesToReturn.ContainsKey(toRemove.ProductId))
					quantitiesToReturn[toRemove.ProductId] = 0;
				quantitiesToReturn[toRemove.ProductId] += toRemove.Quantity;
			}

			// حساب الكميات التي سيتم إرجاعها من التعديلات (عندما تقل الكمية)
			foreach (var item in requestDetails)
			{
				if (item.Id.HasValue && existingDetails.TryGetValue(item.Id.Value, out var existingDetail))
				{
					var quantityDifference = item.Quantity - existingDetail.Quantity;
					if (quantityDifference < 0) // الكمية قلت
					{
						var quantityToReturn = -quantityDifference; // جعلها موجبة
						if (!quantitiesToReturn.ContainsKey(item.ProductId))
							quantitiesToReturn[item.ProductId] = 0;
						quantitiesToReturn[item.ProductId] += quantityToReturn;
					}
				}
			}

			// التحقق من الكميات المطلوبة
			foreach (var item in requestDetails)
			{
				var validationResult = await ValidateQuantityForNormalSales(invoice, item, existingDetails, quantitiesToReturn, cancellationToken);
				if (!validationResult.Success)
					return validationResult;
			}
		}

		// ثانياً: حذف التفاصيل المحذوفة وإرجاع AvailableQty
		foreach (var toRemove in detailsToDelete)
		{
			// للمبيعات العادية: إرجاع AvailableQty
			if (invoice.Type != SalesInvoiceType.Distributor)
			{
				var productBalance = await _productBalanceRepository.GetByProductIdAsync(toRemove.ProductId, cancellationToken);
				if (productBalance != null)
				{
					// إرجاع AvailableQty للكمية المحذوفة
					productBalance.Adjust(0, toRemove.Quantity);
					_productBalanceRepository.Update(productBalance);
				}
			}
			invoice.Details.Remove(toRemove);
		}

		// ثالثاً: تطبيق التغييرات على Details و AvailableQty للسطور المتبقية
		foreach (var item in requestDetails)
		{
			// Update existing detail
			if (item.Id.HasValue && existingDetails.TryGetValue(item.Id.Value, out var toUpdate))
			{
				// تعديل سطر موجود
				if (invoice.Type != SalesInvoiceType.Distributor)
				{
					var productBalance = await _productBalanceRepository.GetByProductIdAsync(item.ProductId, cancellationToken);
					if (productBalance != null)
					{
						// حساب الفرق في الكمية
						var quantityDifference = item.Quantity - toUpdate.Quantity;
						if (quantityDifference != 0)
						{
							// إذا زادت الكمية: إنقاص الفرق من AvailableQty (quantityDifference موجب)
							// إذا قلت الكمية: إرجاع الفرق إلى AvailableQty (quantityDifference سالب، فالإرجاع موجب)
							productBalance.Adjust(0, -quantityDifference);
							_productBalanceRepository.Update(productBalance);
						}
					}
				}
				toUpdate.Update(item.ProductId, item.Quantity, item.UnitPrice, item.SubTotal, item.Notes ?? string.Empty);
			}
			// Add new detail - إنقاص AvailableQty
			else
			{
				// للمبيعات العادية: إنقاص AvailableQty
				if (invoice.Type != SalesInvoiceType.Distributor)
				{
					var productBalance = await _productBalanceRepository.GetByProductIdAsync(item.ProductId, cancellationToken);
					if (productBalance != null)
					{
						// إنقاص AvailableQty للكمية الجديدة
						productBalance.Adjust(0, -item.Quantity);
						_productBalanceRepository.Update(productBalance);
					}
				}

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
		System.Collections.Generic.Dictionary<long, decimal> quantitiesToReturn,
		CancellationToken cancellationToken)
	{
		var productBalance = await _productBalanceRepository.GetByProductIdAsync(item.ProductId, cancellationToken);
		if (productBalance == null)
		{
			return new Error(ErrorCode.NotFound,
				$"رصيد المنتج غير موجود للمنتج {item.ProductId}",
				nameof(UpdateSalesInvoiceCommand.Details));
		}

		// حساب AvailableQty الفعلية بعد أخذ الكميات المرجعة من الحذف والتعديلات في الاعتبار
		var effectiveAvailableQty = productBalance.AvailableQty;
		if (quantitiesToReturn.TryGetValue(item.ProductId, out var returnedQty))
		{
			effectiveAvailableQty += returnedQty;
		}

		// For new items, check if available quantity is sufficient
		if (!item.Id.HasValue || !existingDetails.TryGetValue(item.Id.Value, out var existingDetail))
		{
			if (effectiveAvailableQty < item.Quantity)
			{
				return new Error(ErrorCode.FieldDataInvalid,
					$"الكمية المتاحة غير كافية للمنتج {item.ProductId}. المتاح: {effectiveAvailableQty}, المطلوب: {item.Quantity}",
					nameof(UpdateSalesInvoiceCommand.Details));
			}
		}
		// For existing items, check if the quantity increase is within available stock
		else
		{
			var quantityDifference = item.Quantity - existingDetail.Quantity;
			if (quantityDifference > 0 && effectiveAvailableQty < quantityDifference)
			{
				return new Error(ErrorCode.FieldDataInvalid,
					$"الكمية المتاحة غير كافية للمنتج {item.ProductId}. المتاح: {effectiveAvailableQty}, المطلوب إضافياً: {quantityDifference}",
					nameof(UpdateSalesInvoiceCommand.Details));
			}
		}

		return BaseResult.Ok();
	}
	}
}



