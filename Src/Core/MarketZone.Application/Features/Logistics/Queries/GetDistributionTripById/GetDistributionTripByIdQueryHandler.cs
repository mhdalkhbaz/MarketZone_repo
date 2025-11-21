using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Logistics.DTOs;
using MarketZone.Application.DTOs;

namespace MarketZone.Application.Features.Logistics.Queries.GetDistributionTripById
{
	public class GetDistributionTripByIdQueryHandler(IDistributionTripRepository repository, IMapper mapper, ITranslator translator) : IRequestHandler<GetDistributionTripByIdQuery, BaseResult<DistributionTripDto>>
	{
		public async Task<BaseResult<DistributionTripDto>> Handle(GetDistributionTripByIdQuery request, CancellationToken cancellationToken)
		{
			var trip = await repository.GetWithDetailsByIdAsync(request.Id, cancellationToken);

			if (trip is null)
			{
				return new Error(ErrorCode.NotFound, translator.GetString("DistributionTrip_NotFound"), nameof(request.Id));
			}

			return mapper.Map<DistributionTripDto>(trip);
		}
	}
}

