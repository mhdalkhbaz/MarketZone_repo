using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Logistics.Enums;

namespace MarketZone.Application.Features.Logistics.Commands.DistributionTrips.CompleteTrip
{
	public class CompleteTripCommandHandler : IRequestHandler<CompleteTripCommand, BaseResult>
	{
		private readonly IDistributionTripRepository _repository;
		private readonly IUnitOfWork _unitOfWork;

		public CompleteTripCommandHandler(IDistributionTripRepository repository, IUnitOfWork unitOfWork)
		{
			_repository = repository;
			_unitOfWork = unitOfWork;
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
