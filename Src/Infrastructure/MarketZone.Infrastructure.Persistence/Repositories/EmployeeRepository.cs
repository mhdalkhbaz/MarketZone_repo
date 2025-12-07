using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MarketZone.Application.DTOs;
using MarketZone.Application.Parameters;
using System.Collections.Generic;
using MarketZone.Domain.Employees.DTOs;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Domain.Employees.Entities;
using MarketZone.Domain.Employees.Enums;
using MarketZone.Domain.Cash.Enums;
using MarketZone.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Threading;

namespace MarketZone.Infrastructure.Persistence.Repositories
{
	public class EmployeeRepository(ApplicationDbContext dbContext, IMapper mapper) : GenericRepository<Employee>(dbContext), IEmployeeRepository
	{
		public async Task<PaginationResponseDto<EmployeeDto>> GetPagedListAsync(EmployeeFilter filter)
		{
			var query = dbContext.Set<Employee>().AsQueryable();

			// Apply filters using FilterBuilder pattern
			if (!string.IsNullOrEmpty(filter.Name))
			{
				query = query.Where(p => p.FirstName.Contains(filter.Name) || p.LastName.Contains(filter.Name));
			}

			if (!string.IsNullOrEmpty(filter.Address))
			{
				query = query.Where(p => !string.IsNullOrEmpty(p.Address) && p.Address.Contains(filter.Address));
			}

			if (filter.Status.HasValue)
			{
				// Map status to IsActive: 1 = true, 0 = false
				query = query.Where(p => p.IsActive == (filter.Status.Value == 1));
			}

			if (!string.IsNullOrEmpty(filter.Type))
			{
				query = query.Where(p => p.JobTitle == filter.Type);
			}

			query = query.OrderByDescending(p => p.Created);

			return await Paged(
				query.ProjectTo<EmployeeDto>(mapper.ConfigurationProvider),
				filter.PageNumber,
				filter.PageSize);
		}

		public async Task<List<SelectListDto>> GetActiveSelectListAsync(string? type = null)
		{
			return await dbContext.Set<Employee>()
				.Where(x => x.IsActive && (type == null || x.JobTitle == type))
				.OrderBy(x => x.FirstName)
				.Select(x => new SelectListDto(x.FirstName + " " + x.LastName, x.Id.ToString()))
				.AsNoTracking()
				.ToListAsync();
		}

		public async Task<Dictionary<long, Currency?>> GetEmployeeCurrenciesAsync(List<long> employeeIds, CancellationToken cancellationToken = default)
		{
			if (employeeIds == null || !employeeIds.Any())
				return new Dictionary<long, Currency?>();

			return await dbContext.Employees
				.Where(e => employeeIds.Contains(e.Id))
				.Select(e => new { e.Id, e.Currency })
				.ToDictionaryAsync(e => e.Id, e => e.Currency, cancellationToken);
		}
	}
}



