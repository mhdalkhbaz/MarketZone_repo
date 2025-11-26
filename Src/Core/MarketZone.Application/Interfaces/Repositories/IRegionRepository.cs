using System.Threading.Tasks;
using MarketZone.Application.DTOs;
using MarketZone.Application.Parameters;
using MarketZone.Domain.Logistics.DTOs;
using MarketZone.Domain.Logistics.Entities;

namespace MarketZone.Application.Interfaces.Repositories
{
	public interface IRegionRepository : IGenericRepository<Region>
	{
		Task<PaginationResponseDto<RegionDto>> GetPagedListAsync(RegionFilter filter);
	}
}


