using System;
using MarketZone.Domain.Cash.Enums;
using MarketZone.Domain.Employees.Entities;
using MarketZone.Domain.Employees.Enums;

namespace MarketZone.Domain.Employees.DTOs
{
	public class EmployeeDto
	{
#pragma warning disable
		public EmployeeDto()
		{
		}
#pragma warning restore
		public EmployeeDto(Employee employee)
		{
			Id = employee.Id;
			FirstName = employee.FirstName;
			LastName = employee.LastName;
			Phone = employee.Phone;
			WhatsAppPhone = employee.WhatsAppPhone;
			Email = employee.Email;
			Address = employee.Address;
			JobTitle = employee.JobTitle;
			Salary = employee.Salary;
			HireDate = employee.HireDate;
			IsActive = employee.IsActive;
			SyrianMoney = employee.SyrianMoney;
			DollarMoney = employee.DollarMoney;
			SalaryType = employee.SalaryType;
			SalaryPercentage = employee.SalaryPercentage;
			Currency = employee.Currency;
			CreatedDateTime = employee.Created;
		}

		public long Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Phone { get; set; }
		public string WhatsAppPhone { get; set; }
		public string Email { get; set; }
		public string Address { get; set; }
		public string JobTitle { get; set; }
		public decimal Salary { get; set; }
		public DateTime HireDate { get; set; }
		public bool IsActive { get; set; }
		public decimal? SyrianMoney { get; set; }
		public decimal? DollarMoney { get; set; }
		public SalaryType SalaryType { get; set; }
		public decimal? SalaryPercentage { get; set; }
		public Currency? Currency { get; set; }
		public decimal RemainingSalary { get; set; }  // الرصيد المتبقي من الراتب
		public DateTime CreatedDateTime { get; set; }
	}
}



