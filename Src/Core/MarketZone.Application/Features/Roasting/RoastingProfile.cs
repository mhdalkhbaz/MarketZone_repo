using AutoMapper;
using MarketZone.Application.Features.Roasting.Commands.CreateRoastingInvoice;
using MarketZone.Application.Features.Roasting.Commands.UpdateRoastingInvoice;
using MarketZone.Domain.Roasting.Entities;
using MarketZone.Domain.Roasting.DTOs;

namespace MarketZone.Application.Features.Roasting
{
    public class RoastingProfile : Profile
    {
        public RoastingProfile()
        {
            // RoastingInvoice mappings
            CreateMap<RoastingInvoice, RoastingInvoiceDto>();
            CreateMap<RoastingInvoiceDetail, RoastingInvoiceDetailDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));

            CreateMap<CreateRoastingInvoiceCommand, RoastingInvoice>()
                .ForMember(dest => dest.Details, opt => opt.Ignore());

            CreateMap<UpdateRoastingInvoiceCommand, RoastingInvoice>()
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.PaymentStatus, opt => opt.Ignore())
                .ForMember(dest => dest.Details, opt => opt.Ignore());

            CreateMap<CreateRoastingInvoiceDetailItem, RoastingInvoiceDetail>();
            CreateMap<UpdateRoastingInvoiceDetailItem, RoastingInvoiceDetail>();
        }
    }
}
