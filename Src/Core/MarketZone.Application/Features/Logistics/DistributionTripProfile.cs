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
            CreateMap<DistributionTrip, DistributionTripDto>();
            CreateMap<DistributionTripDetail, DistributionTripDetailDto>();
            CreateMap<CreateDistributionTripDetailItem, DistributionTripDetail>()
                .ConstructUsing(s => new DistributionTripDetail(0, s.ProductId, s.Qty, s.ExpectedPrice.Value));
            CreateMap<CreateDistributionTripCommand, DistributionTrip>()
                .ConstructUsing(s => new DistributionTrip(s.EmployeeId, s.CarId, s.RegionId, s.TripDate, s.LoadQty, s.Notes))
                .ForMember(d => d.Details, opt => opt.Ignore());
            CreateMap<UpdateDistributionTripCommand, DistributionTrip>()
                .ForMember(d => d.Details, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}


