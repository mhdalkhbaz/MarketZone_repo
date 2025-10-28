using System;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Employees.Commands.CreateEmployee
{
	public class CreateEmployeeCommand : IRequest<BaseResult<long>>
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Phone { get; set; }
		public string WhatsAppPhone { get; set; }
		public string Email { get; set; }
		public string Address { get; set; }
		public string JobTitle { get; set; }
		public decimal Salary { get; set; }
		public DateTime HireDate { get; set; }
		public bool IsActive { get; set; } = true;
		public decimal? SyrianMoney { get; set; }
		public decimal? DollarMoney { get; set; }
	}
}



