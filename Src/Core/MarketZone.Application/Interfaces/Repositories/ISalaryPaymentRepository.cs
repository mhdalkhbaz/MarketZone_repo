using System.Collections.Generic;
using System.Threading.Tasks;
using MarketZone.Application.DTOs;
using MarketZone.Domain.Employees.DTOs;

namespace MarketZone.Application.Interfaces.Repositories
{
    public interface ISalaryPaymentRepository : IGenericRepository<MarketZone.Domain.Employees.Entities.SalaryPayment>
    {
        Task<PaginationResponseDto<SalaryPaymentDto>> GetPagedListAsync(int pageNumber, int pageSize, long? employeeId, int? year, int? month);
        Task<List<SalaryPaymentDto>> GetByEmployeeAndMonthAsync(long employeeId, int year, int month);
    }
}
