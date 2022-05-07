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
                .ForMember(dest => dest.RawProductId, opt => opt.MapFrom(src => src.RawProductId))
                .ForMember(dest => dest.RawProductName, opt => opt.MapFrom(src => src.RawProduct != null ? src.RawProduct.Name : null))
                .ForMember(dest => dest.RoastingCost, opt => opt.MapFrom(src => src.RoastingCost))
                .ForMember(dest => dest.ReceivedQuantityKg, opt => opt.MapFrom(src => src.ReceivedQuantityKg))
                .ForMember(dest => dest.RemainingQuantity, opt => opt.MapFrom(src => src.RemainingQuantity));

            CreateMap<RoastingInvoiceDetailReceipt, RoastingInvoiceDetailReceiptDto>()
                //.ForMember(dest => dest.ReadyProductName, opt => opt.MapFrom(src => src.Detail != null && src.ReadyProductId != null ? src.Detail.RawProduct.Name : null))
                .ForMember(dest => dest.TotalSalePrice, opt => opt.MapFrom(src => src.TotalSalePrice))
                .ForMember(dest => dest.TotalRoastingCost, opt => opt.MapFrom(src => src.TotalRoastingCost))
                .ForMember(dest => dest.TotalCommission, opt => opt.MapFrom(src => src.TotalCommission))
                .ForMember(dest => dest.TotalNetSalePrice, opt => opt.MapFrom(src => src.TotalNetSalePrice));

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
