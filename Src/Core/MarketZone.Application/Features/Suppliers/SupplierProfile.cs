using AutoMapper;
using MarketZone.Application.Features.Suppliers.Commands.CreateSupplier;
using MarketZone.Application.Features.Suppliers.Commands.UpdateSupplier;
using MarketZone.Domain.Suppliers.DTOs;
using MarketZone.Domain.Suppliers.Entities;

namespace MarketZone.Application.Features.Suppliers
{
	public class SupplierProfile : Profile
	{
		public SupplierProfile()
		{
			CreateMap<Supplier, SupplierDto>()
				.ForMember(d => d.CreatedDateTime, o => o.MapFrom(s => s.Created));

			CreateMap<CreateSupplierCommand, Supplier>()
				.ConstructUsing(s => new Supplier(s.Name, s.Phone, s.WhatsAppPhone, s.Email, s.Address, s.IsActive));

			CreateMap<UpdateSupplierCommand, Supplier>()
				.ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
		}
	}
}



