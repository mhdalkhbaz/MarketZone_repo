using System.Threading.Tasks;
using MarketZone.Application.DTOs;
using MarketZone.Domain.Categories.DTOs;
using MarketZone.Domain.Categories.Entities;

namespace MarketZone.Application.Interfaces.Repositories
{
	public interface ICategoryRepository : IGenericRepository<Category>
	{
		Task<PaginationResponseDto<CategoryDto>> GetPagedListAsync(int pageNumber, int pageSize, string name);
	}
}


