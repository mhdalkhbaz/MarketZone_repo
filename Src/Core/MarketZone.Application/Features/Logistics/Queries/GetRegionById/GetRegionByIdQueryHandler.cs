using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MarketZone.Application.Helpers;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Logistics.DTOs;

namespace MarketZone.Application.Features.Logistics.Queries.GetRegionById
{
	public class GetRegionByIdQueryHandler(IRegionRepository regionRepository, ITranslator translator, IMapper mapper) : IRequestHandler<GetRegionByIdQuery, BaseResult<RegionDto>>
	{
		public async Task<BaseResult<RegionDto>> Handle(GetRegionByIdQuery request, CancellationToken cancellationToken)
		{
			var region = await regionRepository.GetByIdAsync(request.Id);

			if (region is null)
			{
				return new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.RegionMessages.Region_NotFound_with_id(request.Id)), nameof(request.Id));
			}

			return mapper.Map<RegionDto>(region);
		}
	}
}


