using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Helpers;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Logistics.Commands.Regions.DeleteRegion
{
	public class DeleteRegionCommandHandler(
		IRegionRepository regionRepository,
		IDistributionTripRepository distributionTripRepository,
		IUnitOfWork unitOfWork,
		ITranslator translator) : IRequestHandler<DeleteRegionCommand, BaseResult>
	{
		public async Task<BaseResult> Handle(DeleteRegionCommand request, CancellationToken cancellationToken)
		{
			var region = await regionRepository.GetByIdAsync(request.Id);

			if (region is null)
			{
				return new Error(ErrorCode.NotFound, translator.GetString("Region not found"), nameof(request.Id));
			}

			// Check if there are any DistributionTrips associated with this Region
			var hasDistributionTrips = await distributionTripRepository.HasDistributionTripsAsync(request.Id, cancellationToken);

			if (hasDistributionTrips)
			{
				return new Error(ErrorCode.FieldDataInvalid,
					translator.GetString("Cannot delete region that has associated distribution trips"),
					nameof(request.Id));
			}

			regionRepository.Delete(region);
			await unitOfWork.SaveChangesAsync();

			return BaseResult.Ok();
		}
	}
}

