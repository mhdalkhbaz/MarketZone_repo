using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MarketZone.Application.DTOs;
using MarketZone.Application.Parameters;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Domain.Logistics.DTOs;
using MarketZone.Domain.Logistics.Entities;
using MarketZone.Infrastructure.Persistence.Contexts;

namespace MarketZone.Infrastructure.Persistence.Repositories
{
	public class RegionRepository(ApplicationDbContext dbContext, IMapper mapper) : GenericRepository<Region>(dbContext), IRegionRepository
	{
		public async Task<PaginationResponseDto<RegionDto>> GetPagedListAsync(RegionFilter filter)
		{
			var query = dbContext.Set<Region>().AsQueryable();

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
				query.ProjectTo<RegionDto>(mapper.ConfigurationProvider),
				filter.PageNumber,
				filter.PageSize);
		}
	}
}


