using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MarketZone.Application.DTOs;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Domain.Categories.DTOs;
using MarketZone.Domain.Categories.Entities;
using MarketZone.Infrastructure.Persistence.Contexts;

namespace MarketZone.Infrastructure.Persistence.Repositories
{
	public class CategoryRepository(ApplicationDbContext dbContext, IMapper mapper) : GenericRepository<Category>(dbContext), ICategoryRepository
	{
		public async Task<PaginationResponseDto<CategoryDto>> GetPagedListAsync(int pageNumber, int pageSize, string name)
		{
			var query = dbContext.Set<Category>().OrderByDescending(p => p.Created).AsQueryable();

			if (!string.IsNullOrEmpty(name))
			{
				query = query.Where(p => p.Name.Contains(name));
			}

			return await Paged(
				query.ProjectTo<CategoryDto>(mapper.ConfigurationProvider),
				pageNumber,
				pageSize);
		}
	}
}


