using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketZone.Application.DTOs;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Parameters;
using MarketZone.Domain.Employees.DTOs;
using MarketZone.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace MarketZone.Infrastructure.Persistence.Repositories
{
    public class SalaryPaymentRepository(ApplicationDbContext dbContext) : GenericRepository<MarketZone.Domain.Employees.Entities.SalaryPayment>(dbContext), ISalaryPaymentRepository
    {
        public async Task<PaginationResponseDto<SalaryPaymentDto>> GetPagedListAsync(SalaryPaymentFilter filter)
        {
            var query = dbContext.SalaryPayments
                .Include(sp => sp.Employee)
                .Include(sp => sp.CashRegister)
                .AsQueryable();

            // Apply filters using FilterBuilder pattern
            if (filter.EmployeeId.HasValue)
            {
                query = query.Where(sp => sp.EmployeeId == filter.EmployeeId.Value);
            }

            if (filter.Year.HasValue)
            {
                query = query.Where(sp => sp.Year == filter.Year.Value);
            }

            if (filter.Month.HasValue)
            {
                query = query.Where(sp => sp.Month == filter.Month.Value);
            }

            if (filter.CashRegisterId.HasValue)
            {
                query = query.Where(sp => sp.CashRegisterId == filter.CashRegisterId.Value);
            }

            if (!string.IsNullOrEmpty(filter.Name))
            {
                query = query.Where(sp => sp.Employee.FirstName.Contains(filter.Name) || sp.Employee.LastName.Contains(filter.Name));
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

            return await Paged(pagedQuery, filter.PageNumber, filter.PageSize);
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

        public async Task<MarketZone.Domain.Employees.Entities.SalaryPayment> GetByIdWithIncludesAsync(long id, System.Threading.CancellationToken cancellationToken = default)
        {
            return await dbContext.SalaryPayments
                .Include(sp => sp.Employee)
                .Include(sp => sp.CashRegister)
                .FirstOrDefaultAsync(sp => sp.Id == id, cancellationToken);
        }
    }
}
