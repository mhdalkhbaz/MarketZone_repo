using AutoMapper;
using MarketZone.Application.Features.Customers.Commands.CreateCustomer;
using MarketZone.Application.Features.Customers.Commands.UpdateCustomer;
using MarketZone.Domain.Customers.DTOs;
using MarketZone.Domain.Customers.Entities;

namespace MarketZone.Application.Features.Customers
{
	public class CustomerProfile : Profile
	{
		public CustomerProfile()
		{
			CreateMap<Customer, CustomerDto>()
				.ForMember(d => d.CreatedDateTime, o => o.MapFrom(s => s.Created));

			CreateMap<CreateCustomerCommand, Customer>()
				.ConstructUsing(s => new Customer(s.Name, s.Phone, s.WhatsAppPhone, s.Email, s.Address, s.Currency, s.IsActive));

			CreateMap<UpdateCustomerCommand, Customer>()
				.ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
		}
	}
}



