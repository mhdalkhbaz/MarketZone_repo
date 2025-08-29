using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MarketZone.Application.DTOs;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Domain.Logistics.DTOs;
using MarketZone.Domain.Logistics.Entities;
using MarketZone.Infrastructure.Persistence.Contexts;

namespace MarketZone.Infrastructure.Persistence.Repositories
{
	public class CarRepository(ApplicationDbContext dbContext, IMapper mapper) : GenericRepository<Car>(dbContext), ICarRepository
	{
		public async Task<PaginationResponseDto<CarDto>> GetPagedListAsync(int pageNumber, int pageSize, string name)
		{
			var query = dbContext.Set<Car>().OrderByDescending(p => p.Created).AsQueryable();

			if (!string.IsNullOrEmpty(name))
			{
				query = query.Where(p => p.Name.Contains(name));
			}

			return await Paged(
				query.ProjectTo<CarDto>(mapper.ConfigurationProvider),
				pageNumber,
				pageSize);
		}
	}
}


