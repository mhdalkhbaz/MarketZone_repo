using MarketZone.Domain.Products.Entities;
using MarketZone.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MarketZone.Infrastructure.Persistence.Seeds
{
    public static class DefaultData
    {
        public static async Task SeedAsync(ApplicationDbContext applicationDbContext)
        {
            if (!await applicationDbContext.Products.AnyAsync())
            {
                List<Product> defaultProducts = [
                    new Product(1, "Product 1", null, "kg", null, 100000m, 5m, true, false, null, "111111111111"),
                    new Product(1, "Product 2", null, "kg", null, 150000m, 5m, true, false, null, "222222222222"),
                    new Product(1, "Product 3", null, "kg", null, 200000m, 5m, true, false, null, "333333333333"),
                    new Product(1, "Product 4", null, "kg", null, 105000m, 5m, true, false, null, "444444444444"),
                    new Product(1, "Product 5", null, "kg", null, 200000m, 5m, true, false, null, "555555555555")
                ];

                await applicationDbContext.Products.AddRangeAsync(defaultProducts);

                await applicationDbContext.SaveChangesAsync();
            }
        }
    }
}
