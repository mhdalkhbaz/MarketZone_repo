using System.Collections.Generic;
using System.Threading.Tasks;
using MarketZone.Application.DTOs;
using MarketZone.Domain.Products.DTOs;
using MarketZone.Domain.Products.Entities;

namespace MarketZone.Application.Interfaces.Repositories
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<PaginationResponseDto<ProductDto>> GetPagedListAsync(int pageNumber, int pageSize, string name);
        Task<Dictionary<long, Product>> GetByIdsAsync(IEnumerable<long> ids, System.Threading.CancellationToken cancellationToken = default);
    }
}
