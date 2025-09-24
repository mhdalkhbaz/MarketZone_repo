using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Logistics.DTOs;

namespace MarketZone.Application.Features.Logistics.Queries.GetCarById
{
	public class GetCarByIdQuery : IRequest<BaseResult<CarDto>>
	{
		public long Id { get; set; }
	}
}


