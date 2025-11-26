using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.DTOs;
using MarketZone.Application.Parameters;
using MarketZone.Domain.Logistics.DTOs;
using MarketZone.Domain.Logistics.Entities;

namespace MarketZone.Application.Interfaces.Repositories
{
	public interface IDistributionTripRepository : IGenericRepository<DistributionTrip>
	{
		Task<PaginationResponseDto<DistributionTripDto>> GetPagedListAsync(DistributionTripFilter filter);
		Task<DistributionTrip> GetWithDetailsByIdAsync(long id, CancellationToken cancellationToken = default);
		Task<bool> HasDistributionTripsAsync(long regionId, CancellationToken cancellationToken = default);
		Task<string> GetNextTripNumberAsync(CancellationToken cancellationToken = default);
	}
}


