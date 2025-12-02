using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.DTOs;
using MarketZone.Application.Parameters;
using MarketZone.Domain.Products.DTOs;
using MarketZone.Domain.Products.Entities;

namespace MarketZone.Application.Interfaces.Repositories
{
    public interface ICompositeProductRepository : IGenericRepository<CompositeProduct>
    {
        Task<CompositeProduct> GetWithDetailsByIdAsync(long id, CancellationToken cancellationToken = default);
        Task<PaginationResponseDto<CompositeProductDto>> GetPagedListAsync(CompositeProductFilter filter);
        Task<CompositeProduct> GetByIdsAsync(long id, CancellationToken cancellationToken = default);
    }
}

