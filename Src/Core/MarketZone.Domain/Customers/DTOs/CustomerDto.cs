using System;
using MarketZone.Domain.Customers.Entities;
using MarketZone.Domain.Cash.Enums;

namespace MarketZone.Domain.Customers.DTOs
{
	public class CustomerDto
	{
#pragma warning disable
		public CustomerDto()
		{
		}
#pragma warning restore
		public CustomerDto(Customer customer)
		{
			Id = customer.Id;
			Name = customer.Name;
			Phone = customer.Phone;
			WhatsAppPhone = customer.WhatsAppPhone;
			Email = customer.Email;
			Address = customer.Address;
			Currency = customer.Currency;
			IsActive = customer.IsActive;
			CreatedDateTime = customer.Created;
		}

		public long Id { get; set; }
		public string Name { get; set; }
		public string Phone { get; set; }
		public string WhatsAppPhone { get; set; }
		public string Email { get; set; }
		public string Address { get; set; }
		public Currency? Currency { get; set; }
		public bool IsActive { get; set; }
		public DateTime CreatedDateTime { get; set; }
	}
}



