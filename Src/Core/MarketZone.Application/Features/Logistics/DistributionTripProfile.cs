using AutoMapper;
using MarketZone.Application.Features.Logistics.Commands.DistributionTrips.CreateDistributionTrip;
using MarketZone.Application.Features.Logistics.Commands.DistributionTrips.UpdateDistributionTrip;
using MarketZone.Domain.Logistics.DTOs;
using MarketZone.Domain.Logistics.Entities;

namespace MarketZone.Application.Features.Logistics
{
    public class DistributionTripProfile : Profile
    {
        public DistributionTripProfile()
        {
			CreateMap<DistributionTrip, DistributionTripDto>()
				.ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee != null ? $"{src.Employee.FirstName} {src.Employee.LastName}" : string.Empty))
				.ForMember(dest => dest.CarName, opt => opt.MapFrom(src => src.Car != null ? src.Car.Name : string.Empty))
				.ForMember(dest => dest.RegionName, opt => opt.MapFrom(src => src.Region != null ? src.Region.Name : string.Empty));
            CreateMap<DistributionTripDetail, DistributionTripDetailDto>();
            CreateMap<CreateDistributionTripDetailItem, DistributionTripDetail>()
                .ConstructUsing(s => new DistributionTripDetail(0, s.ProductId, s.Qty, s.ExpectedPrice.Value));
            CreateMap<CreateDistributionTripCommand, DistributionTrip>()
                .ConstructUsing(s => new DistributionTrip(s.EmployeeId, s.CarId, s.RegionId, s.TripDate, s.LoadQty, s.Notes, string.Empty))
                .ForMember(d => d.Details, opt => opt.Ignore())
                .ForMember(d => d.TripNumber, opt => opt.Ignore());
            CreateMap<UpdateDistributionTripCommand, DistributionTrip>()
                .ForMember(d => d.Details, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}


