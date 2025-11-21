using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.DTOs;
using MarketZone.Domain.Logistics.DTOs;
using MarketZone.Domain.Logistics.Entities;

namespace MarketZone.Application.Interfaces.Repositories
{
	public interface IDistributionTripRepository : IGenericRepository<DistributionTrip>
	{
		Task<PaginationResponseDto<DistributionTripDto>> GetPagedListAsync(int pageNumber, int pageSize, long? carId, long? regionId);
		Task<DistributionTrip> GetWithDetailsByIdAsync(long id, CancellationToken cancellationToken = default);
		Task<bool> HasDistributionTripsAsync(long regionId, CancellationToken cancellationToken = default);
	}
}


