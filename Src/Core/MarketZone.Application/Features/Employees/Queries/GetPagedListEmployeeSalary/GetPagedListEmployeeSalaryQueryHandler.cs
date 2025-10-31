using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Employees.DTOs;

namespace MarketZone.Application.Features.Employees.Queries.GetPagedListEmployeeSalary
{
    public class GetPagedListEmployeeSalaryQueryHandler(IEmployeeSalaryRepository employeeSalaryRepository) : IRequestHandler<GetPagedListEmployeeSalaryQuery, PagedResponse<EmployeeSalaryDto>>
    {
        public async Task<PagedResponse<EmployeeSalaryDto>> Handle(GetPagedListEmployeeSalaryQuery request, CancellationToken cancellationToken)
        {
            return await employeeSalaryRepository.GetPagedListAsync(
                request.PageNumber,
                request.PageSize,
                request.EmployeeId,
                request.Year,
                request.Month);
        }
    }
}
