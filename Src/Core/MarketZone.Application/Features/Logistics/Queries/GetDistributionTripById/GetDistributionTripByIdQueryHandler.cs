using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Logistics.DTOs;

namespace MarketZone.Application.Features.Logistics.Queries.GetDistributionTripById
{
	public class GetDistributionTripByIdQueryHandler(IDistributionTripRepository repository, IMapper mapper) : IRequestHandler<GetDistributionTripByIdQuery, BaseResult<DistributionTripDto>>
	{
		public async Task<BaseResult<DistributionTripDto>> Handle(GetDistributionTripByIdQuery request, CancellationToken cancellationToken)
		{
			var trip = await repository.GetWithDetailsByIdAsync(request.Id, cancellationToken);

			if (trip is null)
			{
				return new Error(ErrorCode.NotFound, "Distribution trip not found", nameof(request.Id));
			}

			return mapper.Map<DistributionTripDto>(trip);
		}
	}
}

