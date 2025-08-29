using AutoMapper;
using MarketZone.Application.Features.Logistics.Commands.Regions.CreateRegion;
using MarketZone.Application.Features.Logistics.Commands.Regions.UpdateRegion;
using MarketZone.Domain.Logistics.DTOs;
using MarketZone.Domain.Logistics.Entities;

namespace MarketZone.Application.Features.Logistics
{
	public class RegionProfile : Profile
	{
		public RegionProfile()
		{
			CreateMap<Region, RegionDto>();
			CreateMap<CreateRegionCommand, Region>()
				.ConstructUsing(s => new Region(s.Name, s.Description, s.IsActive));
			CreateMap<UpdateRegionCommand, Region>()
				.ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
		}
	}
}


