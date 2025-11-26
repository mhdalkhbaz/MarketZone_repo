using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Logistics.DTOs;

namespace MarketZone.Application.Features.Logistics.Queries.GetPagedListCar
{
	public class GetPagedListCarQueryHandler(ICarRepository repository) : IRequestHandler<GetPagedListCarQuery, PagedResponse<CarDto>>
	{
		public async Task<PagedResponse<CarDto>> Handle(GetPagedListCarQuery request, CancellationToken cancellationToken)
		{
			var result = await repository.GetPagedListAsync(request);
			return PagedResponse<CarDto>.Ok(result);
		}
	}
}


