using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Logistics.Enums;

namespace MarketZone.Application.Features.Logistics.Commands.DistributionTrips.ReceiveGoods
{
	public class ReceiveGoodsCommandHandler(IDistributionTripRepository repository, IUnitOfWork unitOfWork) : IRequestHandler<ReceiveGoodsCommand, BaseResult>
	{
		public async Task<BaseResult> Handle(ReceiveGoodsCommand request, CancellationToken cancellationToken)
		{
			var trip = await repository.GetWithDetailsByIdAsync(request.TripId, cancellationToken);
			if (trip is null)
				return BaseResult.Failure();

			if (trip.Status != DistributionTripStatus.Posted)
				return BaseResult.Failure();

			// تحديث الكميات المرجعة فقط
			foreach (var item in request.Details)
			{
				var detail = trip.Details.FirstOrDefault(d => d.Id == item.DetailId);
				if (detail != null)
				{
					detail.UpdateReturnedQty(item.ReturnedQty);
				}
			}

			// تحديث حالة الرحلة إلى "تم استلام البضاعة"
			trip.SetStatus(DistributionTripStatus.GoodsReceived);
			await unitOfWork.SaveChangesAsync();
			return BaseResult.Ok();
		}
	}
}
