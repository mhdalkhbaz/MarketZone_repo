using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketZone.Application.DTOs;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Domain.Employees.DTOs;
using MarketZone.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace MarketZone.Infrastructure.Persistence.Repositories
{
    public class SalaryPaymentRepository(ApplicationDbContext dbContext) : GenericRepository<MarketZone.Domain.Employees.Entities.SalaryPayment>(dbContext), ISalaryPaymentRepository
    {
        public async Task<PaginationResponseDto<SalaryPaymentDto>> GetPagedListAsync(int pageNumber, int pageSize, long? employeeId, int? year, int? month)
        {
            var query = dbContext.SalaryPayments
                .Include(sp => sp.Employee)
                .Include(sp => sp.CashRegister)
                .AsQueryable();

            if (employeeId.HasValue)
            {
                query = query.Where(sp => sp.EmployeeId == employeeId.Value);
            }

            if (year.HasValue)
            {
                query = query.Where(sp => sp.Year == year.Value);
            }

            if (month.HasValue)
            {
                query = query.Where(sp => sp.Month == month.Value);
            }

            query = query.OrderByDescending(sp => sp.PaymentDate);

            var pagedQuery = query.Select(sp => new SalaryPaymentDto
            {
                Id = sp.Id,
                EmployeeId = sp.EmployeeId,
                EmployeeName = sp.Employee.FirstName + " " + sp.Employee.LastName,
                Year = sp.Year,
                Month = sp.Month,
                Amount = sp.Amount,
                PaymentDate = sp.PaymentDate,
                CashRegisterId = sp.CashRegisterId,
                CashRegisterName = sp.CashRegister != null ? sp.CashRegister.Name : null,
                Notes = sp.Notes,
                SalaryType = sp.SalaryType,
                DistributionTripId = sp.DistributionTripId,
                CreatedDateTime = sp.Created,
                LastModifiedDateTime = sp.LastModified
            });

            return await Paged(pagedQuery, pageNumber, pageSize);
        }

        public async Task<List<SalaryPaymentDto>> GetByEmployeeAndMonthAsync(long employeeId, int year, int month)
        {
            return await dbContext.SalaryPayments
                .Include(sp => sp.Employee)
                .Include(sp => sp.CashRegister)
                .Where(sp => sp.EmployeeId == employeeId && sp.Year == year && sp.Month == month)
                .OrderByDescending(sp => sp.PaymentDate)
                .Select(sp => new SalaryPaymentDto
                {
                    Id = sp.Id,
                    EmployeeId = sp.EmployeeId,
                    EmployeeName = sp.Employee.FirstName + " " + sp.Employee.LastName,
                    Year = sp.Year,
                    Month = sp.Month,
                    Amount = sp.Amount,
                    PaymentDate = sp.PaymentDate,
                    CashRegisterId = sp.CashRegisterId,
                    CashRegisterName = sp.CashRegister != null ? sp.CashRegister.Name : null,
                    Notes = sp.Notes,
                    SalaryType = sp.SalaryType,
                    DistributionTripId = sp.DistributionTripId,
                    CreatedDateTime = sp.Created,
                    LastModifiedDateTime = sp.LastModified
                })
                .ToListAsync();
        }
    }
}
