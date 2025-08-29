using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MarketZone.Application.DTOs;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Domain.Products.DTOs;
using MarketZone.Domain.Products.Entities;
using MarketZone.Infrastructure.Persistence.Contexts;

namespace MarketZone.Infrastructure.Persistence.Repositories
{
    public class ProductRepository(ApplicationDbContext dbContext, IMapper mapper) : GenericRepository<Product>(dbContext), IProductRepository
    {
        public async Task<PaginationResponseDto<ProductDto>> GetPagedListAsync(int pageNumber, int pageSize, string name)
        {
            var query = dbContext.Products.OrderByDescending(p => p.Created).AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(p => p.Name.Contains(name));
            }

            return await Paged(
                query.ProjectTo<ProductDto>(mapper.ConfigurationProvider),
                pageNumber,
                pageSize);

        }
    }
}
