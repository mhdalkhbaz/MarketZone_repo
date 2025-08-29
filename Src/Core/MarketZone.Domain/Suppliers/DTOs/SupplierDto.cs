using System;
using MarketZone.Domain.Suppliers.Entities;

namespace MarketZone.Domain.Suppliers.DTOs
{
	public class SupplierDto
	{
#pragma warning disable
		public SupplierDto()
		{
		}
#pragma warning restore
		public SupplierDto(Supplier supplier)
		{
			Id = supplier.Id;
			Name = supplier.Name;
			Phone = supplier.Phone;
			WhatsAppPhone = supplier.WhatsAppPhone;
			Email = supplier.Email;
			Address = supplier.Address;
			IsActive = supplier.IsActive;
			CreatedDateTime = supplier.Created;
		}

		public long Id { get; set; }
		public string Name { get; set; }
		public string Phone { get; set; }
		public string WhatsAppPhone { get; set; }
		public string Email { get; set; }
		public string Address { get; set; }
		public bool IsActive { get; set; }
		public DateTime CreatedDateTime { get; set; }
	}
}



