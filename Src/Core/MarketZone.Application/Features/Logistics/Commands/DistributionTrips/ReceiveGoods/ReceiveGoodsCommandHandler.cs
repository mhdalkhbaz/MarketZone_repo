using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Logistics.Enums;
using MarketZone.Domain.Inventory.Entities;
using MarketZone.Application.DTOs;

namespace MarketZone.Application.Features.Logistics.Commands.DistributionTrips.ReceiveGoods
{
	public class ReceiveGoodsCommandHandler : IRequestHandler<ReceiveGoodsCommand, BaseResult>
	{
	private readonly IDistributionTripRepository _repository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly IProductBalanceRepository _productBalanceRepository;
	private readonly IInventoryHistoryRepository _inventoryHistoryRepository;
	private readonly ITranslator _translator;

	public ReceiveGoodsCommandHandler(
		IDistributionTripRepository repository, 
		IUnitOfWork unitOfWork,
		IProductBalanceRepository productBalanceRepository,
		IInventoryHistoryRepository inventoryHistoryRepository,
		ITranslator translator)
	{
		_repository = repository;
		_unitOfWork = unitOfWork;
		_productBalanceRepository = productBalanceRepository;
		_inventoryHistoryRepository = inventoryHistoryRepository;
		_translator = translator;
	}

		public async Task<BaseResult> Handle(ReceiveGoodsCommand request, CancellationToken cancellationToken)
		{
			try
			{
		var trip = await _repository.GetWithDetailsByIdAsync(request.TripId, cancellationToken);
		if (trip is null)
			return new Error(ErrorCode.NotFound, _translator.GetString("DistributionTrip_NotFound"), nameof(request.TripId));

		// التحقق من أن الرحلة في حالة Posted
		if (trip.Status != DistributionTripStatus.Posted)
			return new Error(ErrorCode.FieldDataInvalid, _translator.GetString("Cannot_Receive_Goods_For_Non_Posted_Trip"), nameof(request.TripId));

				//// التحقق من وجود التفاصيل
				//if (request.Details == null || !request.Details.Any())
				//	return new Error(ErrorCode.FieldDataInvalid, "At least one detail is required", nameof(request.Details));

				// تحديث الكميات المرجعة وإرجاعها إلى المخزون
				foreach (var item in request.Details)
				{
			var detail = trip.Details.FirstOrDefault(d => d.Id == item.DetailId);
			if (detail == null)
			{
				var message = _translator.GetString(new TranslatorMessageDto("Trip_Detail_Not_Found", new[] { item.DetailId.ToString() }));
				return new Error(ErrorCode.NotFound, message, nameof(request.Details));
			}

			// التحقق من أن الكمية المرجعة لا تتجاوز الكمية المحملة
			if (item.ReturnedQty > detail.Qty)
			{
				var message = _translator.GetString(new TranslatorMessageDto("Returned_Quantity_Exceeds_Loaded_Quantity", 
					new[] { item.ReturnedQty.ToString(), detail.Qty.ToString(), item.DetailId.ToString() }));
				return new Error(ErrorCode.FieldDataInvalid, message, nameof(request.Details));
			}

					// تحديث الكمية المرجعة في تفاصيل الرحلة
					detail.UpdateReturnedQty(item.ReturnedQty);

					// إرجاع الكميات المرجعة إلى المخزون (استلام الرحلة)
					if (item.ReturnedQty > 0)
					{
				var balance = await _productBalanceRepository.GetByProductIdAsync(detail.ProductId, cancellationToken);
				if (balance == null)
				{
					var message = _translator.GetString(new TranslatorMessageDto("Product_Balance_Not_Found_For_Product", new[] { detail.ProductId.ToString() }));
					return new Error(ErrorCode.FieldDataInvalid, message, nameof(request.Details));
				}

						// إضافة الكمية المرجعة إلى Qty و AvailableQty
						balance.Adjust(item.ReturnedQty, item.ReturnedQty);
						_productBalanceRepository.Update(balance);

						// إنشاء سجل في InventoryHistory لتتبع استلام المرتجعات
						var inventoryHistory = new InventoryHistory(
							detail.ProductId,
							"TripReturn",
							trip.Id,
							item.ReturnedQty,
							trip.TripDate,
							$"Returned from trip {trip.Id}"
						);
						await _inventoryHistoryRepository.AddAsync(inventoryHistory);
					}
				}

				// تغيير حالة الرحلة إلى GoodsReceived
				trip.SetStatus(DistributionTripStatus.GoodsReceived);

				await _unitOfWork.SaveChangesAsync();
				return BaseResult.Ok();
			}
	catch (System.Exception ex)
	{
		// في حالة الخطأ، التراجع عن جميع التغييرات
		await _unitOfWork.RollbackAsync();
		var message = _translator.GetString(new TranslatorMessageDto("Error_Receiving_Goods", new[] { ex.Message }));
		return new Error(ErrorCode.Exception, message, nameof(request.TripId));
	}
		}

	}
}
