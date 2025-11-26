using MarketZone.Application.Interfaces;
using MarketZone.Application.Parameters;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Logistics.DTOs;

namespace MarketZone.Application.Features.Logistics.Queries.GetPagedListCar
{
    public class GetPagedListCarQuery : CarFilter, IRequest<PagedResponse<CarDto>>
    {
    }
}


