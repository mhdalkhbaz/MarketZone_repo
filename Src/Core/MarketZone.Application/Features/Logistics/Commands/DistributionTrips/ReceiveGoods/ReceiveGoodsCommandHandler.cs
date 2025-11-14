using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Logistics.Enums;
using MarketZone.Domain.Inventory.Entities;

namespace MarketZone.Application.Features.Logistics.Commands.DistributionTrips.ReceiveGoods
{
	public class ReceiveGoodsCommandHandler : IRequestHandler<ReceiveGoodsCommand, BaseResult>
	{
		private readonly IDistributionTripRepository _repository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IProductBalanceRepository _productBalanceRepository;
		private readonly IInventoryHistoryRepository _inventoryHistoryRepository;

		public ReceiveGoodsCommandHandler(
			IDistributionTripRepository repository, 
			IUnitOfWork unitOfWork,
			IProductBalanceRepository productBalanceRepository,
			IInventoryHistoryRepository inventoryHistoryRepository)
		{
			_repository = repository;
			_unitOfWork = unitOfWork;
			_productBalanceRepository = productBalanceRepository;
			_inventoryHistoryRepository = inventoryHistoryRepository;
		}

		public async Task<BaseResult> Handle(ReceiveGoodsCommand request, CancellationToken cancellationToken)
		{
			try
			{
				var trip = await _repository.GetWithDetailsByIdAsync(request.TripId, cancellationToken);
				if (trip is null)
					return new Error(ErrorCode.NotFound, "Distribution trip not found", nameof(request.TripId));

				// التحقق من أن الرحلة في حالة Posted
				if (trip.Status != DistributionTripStatus.Posted)
					return new Error(ErrorCode.FieldDataInvalid, "Cannot receive goods for trip that is not in Posted status", nameof(request.TripId));

				//// التحقق من وجود التفاصيل
				//if (request.Details == null || !request.Details.Any())
				//	return new Error(ErrorCode.FieldDataInvalid, "At least one detail is required", nameof(request.Details));

				// تحديث الكميات المرجعة وإرجاعها إلى المخزون
				foreach (var item in request.Details)
				{
					var detail = trip.Details.FirstOrDefault(d => d.Id == item.DetailId);
					if (detail == null)
						return new Error(ErrorCode.NotFound, $"Trip detail not found: {item.DetailId}", nameof(request.Details));

					// التحقق من أن الكمية المرجعة لا تتجاوز الكمية المحملة
					if (item.ReturnedQty > detail.Qty)
						return new Error(ErrorCode.FieldDataInvalid, $"Returned quantity ({item.ReturnedQty}) cannot exceed loaded quantity ({detail.Qty}) for detail {item.DetailId}", nameof(request.Details));

					// تحديث الكمية المرجعة في تفاصيل الرحلة
					detail.UpdateReturnedQty(item.ReturnedQty);

					// إرجاع الكميات المرجعة إلى المخزون (استلام الرحلة)
					if (item.ReturnedQty > 0)
					{
						var balance = await _productBalanceRepository.GetByProductIdAsync(detail.ProductId, cancellationToken);
						if (balance == null)
							return new Error(ErrorCode.FieldDataInvalid, $"Product balance not found for product {detail.ProductId}", nameof(request.Details));

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
				return new Error(ErrorCode.Exception, $"Error receiving goods: {ex.Message}", nameof(request.TripId));
			}
		}

	}
}
