using AutoMapper;
using MarketZone.Application.Features.Purchases.Commands.CreatePurchaseInvoice;
using MarketZone.Application.Features.Purchases.Commands.UpdatePurchaseInvoice;
using MarketZone.Domain.Purchases.DTOs;
using MarketZone.Domain.Purchases.Entities;

namespace MarketZone.Application.Features.Purchases
{
    public class PurchaseInvoiceProfile : Profile
    {
        public PurchaseInvoiceProfile()
        {
            CreateMap<PurchaseInvoice, PurchaseInvoiceDto>();
            CreateMap<PurchaseInvoiceDetail, PurchaseInvoiceDetailDto>();
            CreateMap<CreatePurchaseInvoiceDetailItem, PurchaseInvoiceDetail>()
                .ConstructUsing(s => new PurchaseInvoiceDetail(0, s.ProductId, s.Quantity, s.UnitPrice, s.TotalPrice, s.Notes));

            CreateMap<CreatePurchaseInvoiceCommand, PurchaseInvoice>()
                .ConstructUsing(s => new PurchaseInvoice(
                    s.InvoiceNumber,
                    s.SupplierId,
                    s.InvoiceDate,
                    s.TotalAmount,
                    s.Discount,
                    s.Notes,
                    s.Currency))
                .ForMember(d => d.Details, opt => opt.MapFrom(s => s.Details));

            CreateMap<UpdatePurchaseInvoiceCommand, PurchaseInvoice>()
                .ForMember(d => d.Status, opt => opt.Ignore())
                .ForMember(d => d.PaymentStatus, opt => opt.Ignore())
                .ForMember(d => d.Details, opt => opt.Ignore())
                .ForMember(d => d.Currency, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}



