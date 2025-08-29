using System.Threading.Tasks;
using MarketZone.Application.DTOs;
using MarketZone.Domain.Logistics.DTOs;
using MarketZone.Domain.Logistics.Entities;

namespace MarketZone.Application.Interfaces.Repositories
{
	public interface ICarRepository : IGenericRepository<Car>
	{
		Task<PaginationResponseDto<CarDto>> GetPagedListAsync(int pageNumber, int pageSize, string name);
	}
}


