using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Domain.Products.Entities;
using MarketZone.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace MarketZone.Infrastructure.Persistence.Repositories
{
    public class CompositeProductRepository : GenericRepository<CompositeProduct>, ICompositeProductRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CompositeProductRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CompositeProduct> GetWithDetailsByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<CompositeProduct>()
                .Include(x => x.Details)
                    .ThenInclude(d => d.ComponentProduct)
                .Include(x => x.ResultingProduct)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }
    }
}

