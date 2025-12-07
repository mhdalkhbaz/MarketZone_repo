using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MarketZone.Application.DTOs;
using MarketZone.Application.Parameters;
using System.Collections.Generic;
using MarketZone.Domain.Customers.DTOs;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Domain.Customers.Entities;
using MarketZone.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace MarketZone.Infrastructure.Persistence.Repositories
{
	public class CustomerRepository(ApplicationDbContext dbContext, IMapper mapper) : GenericRepository<Customer>(dbContext), ICustomerRepository
	{
		public async Task<PaginationResponseDto<CustomerDto>> GetPagedListAsync(CustomerFilter filter)
		{
			var query = dbContext.Set<Customer>().AsQueryable();

			// Apply filters using FilterBuilder pattern
			if (!string.IsNullOrEmpty(filter.Name))
			{
				query = query.Where(p => p.Name.Contains(filter.Name));
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

			query = query.OrderByDescending(p => p.Created);

			return await Paged(
				query.ProjectTo<CustomerDto>(mapper.ConfigurationProvider),
				filter.PageNumber,
				filter.PageSize);
		}

		public async Task<List<SelectListDto>> GetActiveSelectListAsync()
		{
			return await dbContext.Set<Customer>()
				.Where(x => x.IsActive)
				.OrderBy(x => x.Name)
				.Select(x => new SelectListDto(x.Name, x.Id.ToString()))
				.AsNoTracking()
				.ToListAsync();
		}
	}
}



