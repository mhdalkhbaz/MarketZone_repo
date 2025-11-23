using AutoMapper;
using MarketZone.Application.Features.Sales.Commands.CreateSalesInvoice;
using MarketZone.Application.Features.Sales.Commands.UpdateSalesInvoice;
using MarketZone.Domain.Sales.DTOs;
using MarketZone.Domain.Sales.Entities;

namespace MarketZone.Application.Features.Sales
{
	public class SalesInvoiceProfile : Profile
	{
		public SalesInvoiceProfile()
		{
			CreateMap<SalesInvoice, SalesInvoiceDto>()
			.ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.Name : string.Empty));
			CreateMap<SalesInvoiceDetail, SalesInvoiceDetailDto>()
				.ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : string.Empty));
			CreateMap<CreateSalesInvoiceCommand, SalesInvoice>()
				.ConstructUsing(s => new SalesInvoice(s.InvoiceNumber, s.CustomerId, s.InvoiceDate, s.TotalAmount, s.Discount, s.PaymentMethod, s.Notes ?? string.Empty, s.Currency))
				.ForMember(dest => dest.Details, opt => opt.Ignore())
				.AfterMap((src, dest) => 
				{
					dest.SetType(src.Type);
				});

			CreateMap<CreateSalesInvoiceDetailItem, SalesInvoiceDetail>()
				.ConstructUsing(s => new SalesInvoiceDetail(0, s.ProductId, s.Quantity, s.UnitPrice, s.SubTotal, s.Notes ?? string.Empty));

			CreateMap<UpdateSalesInvoiceCommand, SalesInvoice>()
				.ForMember(dest => dest.Details, opt => opt.Ignore())
				.ForMember(dest => dest.Currency, opt => opt.Ignore())
				.ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
		}
	}
}



