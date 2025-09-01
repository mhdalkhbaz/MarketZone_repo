using MarketZone.Domain.Inventory.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace MarketZone.Application.Interfaces.Repositories
{
    public interface IProductBalanceRepository : IGenericRepository<ProductBalance>
    {
        Task<ProductBalance> GetByProductIdAsync(long productId, CancellationToken cancellationToken = default);
    }
}

