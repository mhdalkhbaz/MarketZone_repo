using System;
using System.Threading;
using System.Threading.Tasks;

namespace MarketZone.Application.Interfaces.Services
{
	public interface IRoastingService
	{
		Task<long> RoastAsync(long productId, decimal quantityKg, decimal roastPricePerKg, DateTime? roastDate, string notes, CancellationToken cancellationToken = default);
	}
}


