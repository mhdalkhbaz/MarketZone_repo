using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Logistics.Enums;

namespace MarketZone.Application.Features.Logistics.Commands.DistributionTrips.CompleteTrip
{
	public class CompleteTripCommandHandler(IDistributionTripRepository repository, IUnitOfWork unitOfWork) : IRequestHandler<CompleteTripCommand, BaseResult>
	{
		public async Task<BaseResult> Handle(CompleteTripCommand request, CancellationToken cancellationToken)
		{
			var trip = await repository.GetWithDetailsByIdAsync(request.TripId, cancellationToken);
			if (trip is null)
				return BaseResult.Failure();

			if (trip.Status != DistributionTripStatus.GoodsReceived)
				return BaseResult.Failure();

			// التحقق من أن جميع التفاصيل تم تحديثها
			var hasUnprocessedDetails = trip.Details.Any(d => d.SoldQty == 0 && d.ReturnedQty == 0);
			if (hasUnprocessedDetails)
				return BaseResult.Failure();

			// التحقق من أن الكميات المباعة والمرجعة لا تتجاوز الكمية المحملة
			var hasInvalidQuantities = trip.Details.Any(d => (d.SoldQty + d.ReturnedQty) > d.Qty);
			if (hasInvalidQuantities)
				return BaseResult.Failure();

			// تحديث حالة الرحلة إلى "مكتملة"
			trip.SetStatus(DistributionTripStatus.Completed);
			await unitOfWork.SaveChangesAsync();
			return BaseResult.Ok();
		}
	}
}
