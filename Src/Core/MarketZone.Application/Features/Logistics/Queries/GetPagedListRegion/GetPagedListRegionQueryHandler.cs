using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Logistics.DTOs;

namespace MarketZone.Application.Features.Logistics.Queries.GetPagedListRegion
{
	public class GetPagedListRegionQueryHandler(IRegionRepository repository) : IRequestHandler<GetPagedListRegionQuery, PagedResponse<RegionDto>>
	{
		public async Task<PagedResponse<RegionDto>> Handle(GetPagedListRegionQuery request, CancellationToken cancellationToken)
		{
			var paged = await repository.GetPagedListAsync(request.PageNumber, request.PageSize, request.Name);
			return PagedResponse<RegionDto>.Ok(paged);
		}
	}
}


