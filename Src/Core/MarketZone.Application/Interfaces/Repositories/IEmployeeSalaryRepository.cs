using System.Collections.Generic;
using System.Threading.Tasks;
using MarketZone.Application.DTOs;
using MarketZone.Domain.Employees.DTOs;
using MarketZone.Domain.Employees.Entities;

namespace MarketZone.Application.Interfaces.Repositories
{
    public interface IEmployeeSalaryRepository : IGenericRepository<EmployeeSalary>
    {
        Task<EmployeeSalary> GetByEmployeeAndMonthAsync(long employeeId, int year, int month);
        Task<EmployeeSalary> GetOrCreateAsync(long employeeId, int year, int month, decimal baseSalary, decimal? percentageAmount);
        Task<PaginationResponseDto<EmployeeSalaryDto>> GetPagedListAsync(int pageNumber, int pageSize, long? employeeId, int? year, int? month);
    }
}
