using System.Collections.Generic;
using System.Threading.Tasks;
using MarketZone.Application.DTOs;
using MarketZone.Application.Parameters;
using MarketZone.Domain.Products.DTOs;
using MarketZone.Domain.Products.Entities;

namespace MarketZone.Application.Interfaces.Repositories
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<PaginationResponseDto<ProductDto>> GetPagedListAsync(ProductFilter filter);
        Task<Dictionary<long, Product>> GetByIdsAsync(IEnumerable<long> ids, System.Threading.CancellationToken cancellationToken = default);
        Task<List<ProductForCompositeDto>> GetProductsForCompositeAsync(System.Threading.CancellationToken cancellationToken = default);
    }
}
