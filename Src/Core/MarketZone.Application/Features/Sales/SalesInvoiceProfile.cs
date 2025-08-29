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
			CreateMap<SalesInvoice, SalesInvoiceDto>();
			CreateMap<SalesInvoiceDetail, SalesInvoiceDetailDto>();

			CreateMap<CreateSalesInvoiceCommand, SalesInvoice>()
				.ConstructUsing(s => new SalesInvoice(s.InvoiceNumber, s.CustomerId, s.InvoiceDate, s.TotalAmount, s.Discount, s.PaymentMethod, s.Notes));

			CreateMap<UpdateSalesInvoiceCommand, SalesInvoice>()
				.ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
		}
	}
}



