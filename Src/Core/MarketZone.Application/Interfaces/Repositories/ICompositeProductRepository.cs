using System.Threading;
using System.Threading.Tasks;
using MarketZone.Domain.Products.Entities;

namespace MarketZone.Application.Interfaces.Repositories
{
    public interface ICompositeProductRepository : IGenericRepository<CompositeProduct>
    {
        Task<CompositeProduct> GetWithDetailsByIdAsync(long id, CancellationToken cancellationToken = default);
    }
}

