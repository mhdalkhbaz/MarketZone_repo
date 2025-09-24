using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Domain.Cash.Entities;
using MarketZone.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace MarketZone.Infrastructure.Persistence.Repositories
{
	public class ExchangeRateRepository : GenericRepository<ExchangeRate>, IExchangeRateRepository
	{
		private readonly ApplicationDbContext _db;
		public ExchangeRateRepository(ApplicationDbContext db) : base(db)
		{
			_db = db;
		}

		public async Task<ExchangeRate> GetLatestAsync(string currencyCode, DateTime atUtc, CancellationToken cancellationToken = default)
		{
			currencyCode = currencyCode?.Trim().ToUpperInvariant();
			return await _db.Set<ExchangeRate>()
				.Where(x => x.CurrencyCode == currencyCode && x.EffectiveAtUtc <= atUtc)
				.OrderByDescending(x => x.EffectiveAtUtc)
				.FirstOrDefaultAsync(cancellationToken);
		}
	}
}


