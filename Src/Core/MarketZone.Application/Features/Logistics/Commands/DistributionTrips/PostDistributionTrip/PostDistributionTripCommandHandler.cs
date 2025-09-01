using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Logistics.Enums;

namespace MarketZone.Application.Features.Logistics.Commands.DistributionTrips.PostDistributionTrip
{
	public class PostDistributionTripCommandHandler(IDistributionTripRepository repository, IUnitOfWork unitOfWork) : IRequestHandler<PostDistributionTripCommand, BaseResult>
	{
		public async Task<BaseResult> Handle(PostDistributionTripCommand request, CancellationToken cancellationToken)
		{
			var trip = await repository.GetByIdAsync(request.Id);
			if (trip is null)
				return BaseResult.Failure();

			if (trip.Status == DistributionTripStatus.Posted)
				return BaseResult.Ok();

			trip.SetStatus(DistributionTripStatus.Posted);
			await unitOfWork.SaveChangesAsync();
			return BaseResult.Ok();
		}
	}
}
