using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Employees.DTOs;
using MarketZone.Domain.Employees.Entities;

namespace MarketZone.Application.Features.Employees.Queries.GetEmployeesWithRemainingSalary
{
    public class GetEmployeesWithRemainingSalaryQueryHandler(
        IEmployeeRepository employeeRepository,
        IEmployeeSalaryRepository employeeSalaryRepository) : IRequestHandler<GetEmployeesWithRemainingSalaryQuery, BaseResult<List<EmployeeDto>>>
    {
        public async Task<BaseResult<List<EmployeeDto>>> Handle(GetEmployeesWithRemainingSalaryQuery request, CancellationToken cancellationToken)
        {
            var year = request.Year ?? DateTime.Now.Year;
            var month = request.Month ?? DateTime.Now.Month;

            // جلب جميع الموظفين النشطين
            var employees = await employeeRepository.GetAllAsync();

            var result = new List<EmployeeDto>();

            foreach (var employee in employees.Where(e => e.IsActive&& e.JobTitle.ToString() != "roasting"))
            {
                var employeeDto = new EmployeeDto(employee);

                // الحصول على EmployeeSalary للشهر المحدد
                var employeeSalary = await employeeSalaryRepository.GetByEmployeeAndMonthAsync(
                    employee.Id,
                    year,
                    month);

                if (employeeSalary != null)
                {
                    // إذا كان السطر موجوداً، نستخدم RemainingAmount
                    employeeDto.RemainingSalary = employeeSalary.RemainingAmount;
                }
                else
                {
                    // إذا لم يكن السطر موجوداً، نستخدم الراتب الثابت من Employee.Salary
                    employeeDto.RemainingSalary = employee.Salary;
                }

                // إضافة الموظف فقط إذا كان RemainingSalary > 0
                if (employeeDto.RemainingSalary > 0)
                {
                    result.Add(employeeDto);
                }
            }

            return BaseResult<List<EmployeeDto>>.Ok(result);
        }
    }
}
