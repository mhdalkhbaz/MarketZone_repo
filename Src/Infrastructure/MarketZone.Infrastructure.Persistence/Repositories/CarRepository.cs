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
	public class CarRepository(ApplicationDbContext dbContext, IMapper mapper) : GenericRepository<Car>(dbContext), ICarRepository
	{
		public async Task<PaginationResponseDto<CarDto>> GetPagedListAsync(CarFilter filter)
		{
			var query = dbContext.Set<Car>().AsQueryable();

			// Apply filters using FilterBuilder pattern
			if (!string.IsNullOrEmpty(filter.Name))
			{
				query = query.Where(p => p.Name.Contains(filter.Name));
			}

			if (!string.IsNullOrEmpty(filter.Description))
			{
				query = query.Where(p => !string.IsNullOrEmpty(p.Model) && p.Model.Contains(filter.Description));
			}

			if (filter.Status.HasValue)
			{
				// Map status to IsAvailable: 1 = true, 0 = false
				query = query.Where(p => p.IsAvailable == (filter.Status.Value == 1));
			}

			query = query.OrderByDescending(p => p.Created);

			return await Paged(
				query.ProjectTo<CarDto>(mapper.ConfigurationProvider),
				filter.PageNumber,
				filter.PageSize);
		}
	}
}


