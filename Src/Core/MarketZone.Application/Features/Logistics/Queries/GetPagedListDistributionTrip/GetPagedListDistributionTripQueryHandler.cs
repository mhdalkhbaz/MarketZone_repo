using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Logistics.DTOs;

namespace MarketZone.Application.Features.Logistics.Queries.GetPagedListDistributionTrip
{
	public class GetPagedListDistributionTripQueryHandler(IDistributionTripRepository repository) : IRequestHandler<GetPagedListDistributionTripQuery, PagedResponse<DistributionTripDto>>
	{
		public async Task<PagedResponse<DistributionTripDto>> Handle(GetPagedListDistributionTripQuery request, CancellationToken cancellationToken)
		{
			var paged = await repository.GetPagedListAsync(request.PageNumber, request.PageSize, request.CarId, request.RegionId);
			return PagedResponse<DistributionTripDto>.Ok(paged);
		}
	}
}


