using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MarketZone.Application.DTOs;
using MarketZone.Application.Parameters;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Domain.Categories.DTOs;
using MarketZone.Domain.Categories.Entities;
using MarketZone.Infrastructure.Persistence.Contexts;

namespace MarketZone.Infrastructure.Persistence.Repositories
{
	public class CategoryRepository(ApplicationDbContext dbContext, IMapper mapper) : GenericRepository<Category>(dbContext), ICategoryRepository
	{
		public async Task<PaginationResponseDto<CategoryDto>> GetPagedListAsync(CategoryFilter filter)
		{
			var query = dbContext.Set<Category>().AsQueryable();

			// Apply filters using FilterBuilder pattern
			if (!string.IsNullOrEmpty(filter.Name))
			{
				query = query.Where(p => p.Name.Contains(filter.Name));
			}

			if (!string.IsNullOrEmpty(filter.Description))
			{
				query = query.Where(p => !string.IsNullOrEmpty(p.Description) && p.Description.Contains(filter.Description));
			}

			query = query.OrderByDescending(p => p.Created);

			return await Paged(
				query.ProjectTo<CategoryDto>(mapper.ConfigurationProvider),
				filter.PageNumber,
				filter.PageSize);
		}
	}
}


