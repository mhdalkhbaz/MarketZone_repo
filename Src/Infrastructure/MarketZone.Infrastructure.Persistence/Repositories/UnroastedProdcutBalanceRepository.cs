using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Domain.Inventory.Entities;
using MarketZone.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MarketZone.Infrastructure.Persistence.Repositories
{
    public class UnroastedProdcutBalanceRepository : GenericRepository<UnroastedProdcutBalance>, IUnroastedProdcutBalanceRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public UnroastedProdcutBalanceRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UnroastedProdcutBalance> GetByProductIdAsync(long productId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<UnroastedProdcutBalance>()
                .FirstOrDefaultAsync(u => u.ProductId == productId, cancellationToken);
        }

        public async Task<Dictionary<long, UnroastedProdcutBalance>> GetByProductIdsAsync(IEnumerable<long> productIds, CancellationToken cancellationToken = default)
        {
            var list = await _dbContext.Set<UnroastedProdcutBalance>()
                .Where(u => productIds.Contains(u.ProductId))
                .ToListAsync(cancellationToken);
            return list.ToDictionary(u => u.ProductId, u => u);
        }
    }
} 