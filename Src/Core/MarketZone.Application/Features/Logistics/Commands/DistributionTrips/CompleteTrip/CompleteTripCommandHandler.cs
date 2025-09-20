using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Logistics.Enums;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Domain.Inventory.Entities;

namespace MarketZone.Application.Features.Logistics.Commands.DistributionTrips.CompleteTrip
{
	public class CompleteTripCommandHandler : IRequestHandler<CompleteTripCommand, BaseResult>
	{
        private readonly IDistributionTripRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductBalanceRepository _productBalanceRepository;

        public CompleteTripCommandHandler(IDistributionTripRepository repository, IUnitOfWork unitOfWork, IProductBalanceRepository productBalanceRepository)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _productBalanceRepository = productBalanceRepository;
        }

		public async Task<BaseResult> Handle(CompleteTripCommand request, CancellationToken cancellationToken)
		{
			try
			{
				var trip = await _repository.GetWithDetailsByIdAsync(request.TripId, cancellationToken);
				if (trip is null)
					return new Error(ErrorCode.NotFound, "Distribution trip not found", nameof(request.TripId));

				if (trip.Status != DistributionTripStatus.GoodsReceived)
					return new Error(ErrorCode.FieldDataInvalid, "Cannot complete trip that is not in GoodsReceived status", nameof(request.TripId));

				// التحقق من أن جميع التفاصيل تم تحديثها
				var hasUnprocessedDetails = trip.Details.Any(d => d.SoldQty == 0 && d.ReturnedQty == 0);
				if (hasUnprocessedDetails)
					return new Error(ErrorCode.FieldDataInvalid, "All trip details must have either sold quantity or returned quantity", nameof(request.TripId));

				// التحقق من أن الكميات المباعة والمرجعة لا تتجاوز الكمية المحملة
				var hasInvalidQuantities = trip.Details.Any(d => (d.SoldQty + d.ReturnedQty) > d.Qty);
				if (hasInvalidQuantities)
				{
					var invalidDetails = trip.Details.Where(d => (d.SoldQty + d.ReturnedQty) > d.Qty).ToList();
					var errorMessage = string.Join("; ", invalidDetails.Select(d => 
						$"Product {d.ProductId}: Loaded={d.Qty}, Sold={d.SoldQty}, Returned={d.ReturnedQty}, Total={d.SoldQty + d.ReturnedQty}"));
					return new Error(ErrorCode.FieldDataInvalid, $"Invalid quantities: {errorMessage}", nameof(request.TripId));
				}

                // تطبيق التأثيرات على المخزون حسب القواعد الجديدة
                // 1) إرجاع المرتجع إلى AvailableQty
                // 2) إنقاص المباع من Qty
                if (request.Details == null || !request.Details.Any())
                    return new Error(ErrorCode.FieldDataInvalid, "At least one detail is required", nameof(request.Details));

                foreach (var item in request.Details)
                {
                    var detail = trip.Details.FirstOrDefault(d => d.Id == item.DetailId);
                    if (detail == null)
                        return new Error(ErrorCode.NotFound, $"Trip detail not found: {item.DetailId}", nameof(request.Details));

                    // 1) إرجاع المرتجع إلى AvailableQty
                    if (item.ReturnedQty > 0)
                    {
                        var balance = await _productBalanceRepository.GetByProductIdAsync(detail.ProductId, cancellationToken);
                        if (balance == null)
                            return new Error(ErrorCode.FieldDataInvalid, $"Product balance not found for product {detail.ProductId}", nameof(request.TripId));

                        balance.Adjust(0, item.ReturnedQty); // زيادة المتاح فقط
                        detail.UpdateReturnedQty(item.ReturnedQty);
                    }

                    // 2) إنقاص المباع من Qty
                    if (detail.SoldQty > 0)
                    {
                        var balance = await _productBalanceRepository.GetByProductIdAsync(detail.ProductId, cancellationToken);
                        if (balance == null)
                            return new Error(ErrorCode.FieldDataInvalid, $"Product balance not found for product {detail.ProductId}", nameof(request.TripId));

                        if (balance.Qty < detail.SoldQty)
                            return new Error(ErrorCode.FieldDataInvalid, $"Insufficient quantity for product {detail.ProductId}. Qty: {balance.Qty}, Sold: {detail.SoldQty}", nameof(request.TripId));

                        balance.Adjust(-detail.SoldQty, 0); // إنقاص الموجودة فقط
                    }
                }

                // تحديث حالة الرحلة إلى "مكتملة"
                trip.SetStatus(DistributionTripStatus.Completed);
                await _unitOfWork.SaveChangesAsync();
				return BaseResult.Ok();
			}
			catch (System.Exception ex)
			{
				// في حالة الخطأ، التراجع عن جميع التغييرات
				await _unitOfWork.RollbackAsync();
				return new Error(ErrorCode.Exception, $"Error completing distribution trip: {ex.Message}", nameof(request.TripId));
			}
		}
	}
}
