using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MarketZone.Application.DTOs;
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
		public async Task<PaginationResponseDto<CustomerDto>> GetPagedListAsync(int pageNumber, int pageSize, string name)
		{
			var query = dbContext.Set<Customer>().OrderByDescending(p => p.Created).AsQueryable();

			if (!string.IsNullOrEmpty(name))
			{
				query = query.Where(p => p.Name.Contains(name));
			}

			return await Paged(
				query.ProjectTo<CustomerDto>(mapper.ConfigurationProvider),
				pageNumber,
				pageSize);
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



