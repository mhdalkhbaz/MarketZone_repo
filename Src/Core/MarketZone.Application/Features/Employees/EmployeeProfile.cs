using AutoMapper;
using MarketZone.Application.Features.Employees.Commands.CreateEmployee;
using MarketZone.Application.Features.Employees.Commands.UpdateEmployee;
using MarketZone.Domain.Employees.DTOs;
using MarketZone.Domain.Employees.Entities;

namespace MarketZone.Application.Features.Employees
{
	public class EmployeeProfile : Profile
	{
		public EmployeeProfile()
		{
			CreateMap<Employee, EmployeeDto>()
				.ForMember(d => d.CreatedDateTime, o => o.MapFrom(s => s.Created));

			CreateMap<CreateEmployeeCommand, Employee>()
				.ConstructUsing(s => new Employee(s.FirstName, s.LastName, s.Phone, s.WhatsAppPhone, s.Email, s.Address, s.JobTitle, s.Salary, s.HireDate, s.IsActive, s.SyrianMoney, s.DollarMoney, s.SalaryType, s.SalaryPercentage,s.Currency));

			CreateMap<UpdateEmployeeCommand, Employee>()
				.ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
		}
	}
}



