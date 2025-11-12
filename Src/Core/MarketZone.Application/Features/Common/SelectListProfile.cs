using AutoMapper;
using MarketZone.Application.DTOs;
using MarketZone.Domain.Customers.Entities;
using MarketZone.Domain.Employees.Entities;
using MarketZone.Domain.Products.Entities;
using MarketZone.Domain.Suppliers.Entities;

namespace MarketZone.Application.Features.Common
{
    public class SelectListProfile : Profile
    {
        public SelectListProfile()
        {
            CreateMap<Product, SelectListDto>()
                .ForMember(d => d.Value, opt => opt.MapFrom(s => s.Id.ToString()))
                .ForMember(d => d.Label, opt => opt.MapFrom(s => s.Name));

            CreateMap<Customer, SelectListDto>()
                .ForMember(d => d.Value, opt => opt.MapFrom(s => s.Id.ToString()))
                .ForMember(d => d.Label, opt => opt.MapFrom(s => s.Name));

            CreateMap<Supplier, SelectListDto>()
                .ForMember(d => d.Value, opt => opt.MapFrom(s => s.Id.ToString()))
                .ForMember(d => d.Label, opt => opt.MapFrom(s => s.Name));

            CreateMap<Employee, SelectListDto>()
                .ForMember(d => d.Value, opt => opt.MapFrom(s => s.Id.ToString()))
                .ForMember(d => d.Label, opt => opt.MapFrom(s => s.FirstName + s.LastName));
        }
    }
}


