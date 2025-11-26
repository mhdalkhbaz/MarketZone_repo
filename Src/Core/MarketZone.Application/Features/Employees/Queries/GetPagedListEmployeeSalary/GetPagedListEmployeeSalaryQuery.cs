using MarketZone.Application.Interfaces;
using MarketZone.Application.Parameters;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Employees.DTOs;

namespace MarketZone.Application.Features.Employees.Queries.GetPagedListEmployeeSalary
{
    public class GetPagedListEmployeeSalaryQuery : EmployeeSalaryFilter, IRequest<PagedResponse<EmployeeSalaryDto>>
    {
    }
}
