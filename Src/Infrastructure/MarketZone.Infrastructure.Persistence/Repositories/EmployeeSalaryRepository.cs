using System.Linq;
using System.Threading.Tasks;
using MarketZone.Application.DTOs;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Domain.Employees.DTOs;
using MarketZone.Domain.Employees.Entities;
using MarketZone.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace MarketZone.Infrastructure.Persistence.Repositories
{
    public class EmployeeSalaryRepository(ApplicationDbContext dbContext) : GenericRepository<EmployeeSalary>(dbContext), IEmployeeSalaryRepository
    {
        public async Task<EmployeeSalary> GetByEmployeeAndMonthAsync(long employeeId, int year, int month)
        {
            return await dbContext.EmployeeSalaries
                .FirstOrDefaultAsync(es => es.EmployeeId == employeeId && es.Year == year && es.Month == month);
        }

        public async Task<EmployeeSalary> GetOrCreateAsync(long employeeId, int year, int month, decimal baseSalary, decimal? percentageAmount)
        {
            var existing = await GetByEmployeeAndMonthAsync(employeeId, year, month);
            
            if (existing != null)
            {
                return existing;
            }

            // Create new EmployeeSalary record
            var newSalary = new EmployeeSalary(employeeId, year, month, baseSalary,percentageAmount);
            await dbContext.EmployeeSalaries.AddAsync(newSalary);
            await dbContext.SaveChangesAsync();

            return newSalary;
        }

        public async Task<PaginationResponseDto<EmployeeSalaryDto>> GetPagedListAsync(int pageNumber, int pageSize, long? employeeId, int? year, int? month)
        {
            var query = dbContext.EmployeeSalaries
                .Include(es => es.Employee)
                .AsQueryable();

            if (employeeId.HasValue)
            {
                query = query.Where(es => es.EmployeeId == employeeId.Value);
            }

            if (year.HasValue)
            {
                query = query.Where(es => es.Year == year.Value);
            }

            if (month.HasValue)
            {
                query = query.Where(es => es.Month == month.Value);
            }

            query = query.OrderByDescending(es => es.Year)
                .ThenByDescending(es => es.Month)
                .ThenBy(es => es.Employee.FirstName + " " + es.Employee.LastName);

            var pagedQuery = query.Select(es => new EmployeeSalaryDto
            {
                Id = es.Id,
                EmployeeId = es.EmployeeId,
                EmployeeName = es.Employee.FirstName + " " + es.Employee.LastName,
                Year = es.Year,
                Month = es.Month,
                BaseSalary = es.BaseSalary,
                PercentageAmount = es.PercentageAmount,
                PaidAmount = es.PaidAmount,
                TotalSalary = es.TotalSalary,
                RemainingAmount = es.RemainingAmount,
                CreatedDateTime = es.Created,
                LastModifiedDateTime = es.LastModified
            });

            return await Paged(pagedQuery, pageNumber, pageSize);
        }
    }
}
