using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MarketZone.Application.DTOs;
using System.Collections.Generic;
using MarketZone.Domain.Employees.DTOs;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Domain.Employees.Entities;
using MarketZone.Domain.Employees.Enums;
using MarketZone.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace MarketZone.Infrastructure.Persistence.Repositories
{
	public class EmployeeRepository(ApplicationDbContext dbContext, IMapper mapper) : GenericRepository<Employee>(dbContext), IEmployeeRepository
	{
		public async Task<PaginationResponseDto<EmployeeDto>> GetPagedListAsync(int pageNumber, int pageSize, string name)
		{
			var query = dbContext.Set<Employee>().OrderByDescending(p => p.Created).AsQueryable();

			if (!string.IsNullOrEmpty(name))
			{
				query = query.Where(p => p.FirstName.Contains(name) || p.LastName.Contains(name));
			}

			return await Paged(
				query.ProjectTo<EmployeeDto>(mapper.ConfigurationProvider),
				pageNumber,
				pageSize);
		}

		public async Task<List<SelectListDto>> GetActiveSelectListAsync(SalaryType? type = null)
		{
			return await dbContext.Set<Employee>()
				.Where(x => x.IsActive && (!type.HasValue || x.SalaryType == type.Value))
				.OrderBy(x => x.FirstName)
				.Select(x => new SelectListDto(x.FirstName + " " + x.LastName, x.Id.ToString()))
				.AsNoTracking()
				.ToListAsync();
		}
	}
}



