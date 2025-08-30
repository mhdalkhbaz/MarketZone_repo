using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Domain.Inventory.Entities;
using MarketZone.Infrastructure.Persistence.Contexts;

namespace MarketZone.Infrastructure.Persistence.Repositories
{
    public class InventoryHistoryRepository : GenericRepository<InventoryHistory>, IInventoryHistoryRepository
    {
        public InventoryHistoryRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
