using AutoMapper;
using MarketZone.Application.Features.Logistics.Commands.CreateCar;
using MarketZone.Application.Features.Logistics.Commands.UpdateCar;
using MarketZone.Domain.Logistics.DTOs;
using MarketZone.Domain.Logistics.Entities;

namespace MarketZone.Application.Features.Logistics
{
	public class CarProfile : Profile
	{
		public CarProfile()
		{
			CreateMap<Car, CarDto>();
			CreateMap<CreateCarCommand, Car>()
				.ConstructUsing(s => new Car(s.Name, s.PlateNumber, s.Model, s.Year, s.CapacityKg, s.IsAvailable));
			CreateMap<UpdateCarCommand, Car>()
				.ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
		}
	}
}


