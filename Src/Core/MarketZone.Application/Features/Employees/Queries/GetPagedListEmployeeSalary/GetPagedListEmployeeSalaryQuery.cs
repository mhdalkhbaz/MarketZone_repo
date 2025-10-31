using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Employees.DTOs;

namespace MarketZone.Application.Features.Employees.Queries.GetPagedListEmployeeSalary
{
    public class GetPagedListEmployeeSalaryQuery : IRequest<PagedResponse<EmployeeSalaryDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public long? EmployeeId { get; set; }
        public int? Year { get; set; }
        public int? Month { get; set; }
    }
}
