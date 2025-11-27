using System.Collections.Generic;
using System.Threading.Tasks;
using MarketZone.Application.DTOs;
using MarketZone.Application.Parameters;
using MarketZone.Domain.Employees.DTOs;

namespace MarketZone.Application.Interfaces.Repositories
{
    public interface ISalaryPaymentRepository : IGenericRepository<MarketZone.Domain.Employees.Entities.SalaryPayment>
    {
        Task<PaginationResponseDto<SalaryPaymentDto>> GetPagedListAsync(SalaryPaymentFilter filter);
        Task<List<SalaryPaymentDto>> GetByEmployeeAndMonthAsync(long employeeId, int year, int month);
        Task<MarketZone.Domain.Employees.Entities.SalaryPayment> GetByIdWithIncludesAsync(long id, System.Threading.CancellationToken cancellationToken = default);
    }
}
