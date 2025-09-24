using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MarketZone.Application.Helpers;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Logistics.DTOs;

namespace MarketZone.Application.Features.Logistics.Queries.GetCarById
{
	public class GetCarByIdQueryHandler(ICarRepository carRepository, ITranslator translator, IMapper mapper) : IRequestHandler<GetCarByIdQuery, BaseResult<CarDto>>
	{
		public async Task<BaseResult<CarDto>> Handle(GetCarByIdQuery request, CancellationToken cancellationToken)
		{
			var car = await carRepository.GetByIdAsync(request.Id);

			if (car is null)
			{
				return new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.CarMessages.Car_NotFound_with_id(request.Id)), nameof(request.Id));
			}

			return mapper.Map<CarDto>(car);
		}
	}
}


