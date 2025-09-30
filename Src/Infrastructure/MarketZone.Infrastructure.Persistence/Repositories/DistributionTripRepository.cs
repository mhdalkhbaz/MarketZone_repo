using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MarketZone.Application.DTOs;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Domain.Logistics.DTOs;
using MarketZone.Domain.Logistics.Entities;
using MarketZone.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace MarketZone.Infrastructure.Persistence.Repositories
{
	public class DistributionTripRepository(ApplicationDbContext dbContext, IMapper mapper) : GenericRepository<DistributionTrip>(dbContext), IDistributionTripRepository
	{
		public async Task<PaginationResponseDto<DistributionTripDto>> GetPagedListAsync(int pageNumber, int pageSize, long? carId, long? regionId)
		{
			var query = dbContext.Set<DistributionTrip>()
				.Include(x => x.Details)
				.OrderByDescending(p => p.TripDate)
				.AsQueryable();

			if (carId.HasValue)
				query = query.Where(p => p.CarId == carId.Value);
			if (regionId.HasValue)
				query = query.Where(p => p.RegionId == regionId.Value);

			return await Paged(
				query.ProjectTo<DistributionTripDto>(mapper.ConfigurationProvider),
				pageNumber,
				pageSize);
		}

		public async Task<DistributionTrip> GetWithDetailsByIdAsync(long id, CancellationToken cancellationToken = default)
		{
			return await dbContext.Set<DistributionTrip>()
				.Include(x => x.Details)
				.ThenInclude(x => x.Product)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
		}
	}
}


