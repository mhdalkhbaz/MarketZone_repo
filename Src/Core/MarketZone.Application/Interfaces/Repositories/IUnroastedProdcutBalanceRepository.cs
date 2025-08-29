using MarketZone.Domain.Inventory.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MarketZone.Application.Interfaces.Repositories
{
    public interface IUnroastedProdcutBalanceRepository : IGenericRepository<UnroastedProdcutBalance>
    {
        Task<UnroastedProdcutBalance> GetByProductIdAsync(long productId, CancellationToken cancellationToken = default);
        Task<Dictionary<long, UnroastedProdcutBalance>> GetByProductIdsAsync(IEnumerable<long> productIds, CancellationToken cancellationToken = default);
    }
} 