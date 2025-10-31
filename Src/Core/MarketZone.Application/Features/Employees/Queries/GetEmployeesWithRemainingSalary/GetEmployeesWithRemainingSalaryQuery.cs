using System.Collections.Generic;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Employees.DTOs;

namespace MarketZone.Application.Features.Employees.Queries.GetEmployeesWithRemainingSalary
{
    public class GetEmployeesWithRemainingSalaryQuery : IRequest<BaseResult<List<EmployeeDto>>>
    {
        public int? Year { get; set; }
        public int? Month { get; set; }
    }
}
