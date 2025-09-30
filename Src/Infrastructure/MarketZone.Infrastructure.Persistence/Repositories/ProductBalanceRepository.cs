using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Domain.Inventory.Entities;
using MarketZone.Domain.Products.Entities;
using MarketZone.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MarketZone.Infrastructure.Persistence.Repositories
{
    public class ProductBalanceRepository : GenericRepository<ProductBalance>, IProductBalanceRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ProductBalanceRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<ProductBalance>> GetAllProductBalanceAsync()
        {
            return await _dbContext.Set<ProductBalance>().Include(x=>x.Product).ToListAsync();
        }

        public async Task<ProductBalance> GetByProductIdAsync(long productId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<ProductBalance>()
                .FirstOrDefaultAsync(b => b.ProductId == productId, cancellationToken);
        }
    }
}

