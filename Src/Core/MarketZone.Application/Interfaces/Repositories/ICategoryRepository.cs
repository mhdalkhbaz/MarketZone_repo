using System.Threading.Tasks;
using MarketZone.Application.DTOs;
using MarketZone.Application.Parameters;
using MarketZone.Domain.Categories.DTOs;
using MarketZone.Domain.Categories.Entities;

namespace MarketZone.Application.Interfaces.Repositories
{
	public interface ICategoryRepository : IGenericRepository<Category>
	{
		Task<PaginationResponseDto<CategoryDto>> GetPagedListAsync(CategoryFilter filter);
	}
}


