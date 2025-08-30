using MarketZone.Domain.Products.Entities;
using MarketZone.Domain.Categories.Entities;
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
            // Seed Categories
            if (!await applicationDbContext.Categories.AnyAsync())
            {
                var defaultCategory = new Category("البن والقهوة", "فئة البن والقهوة بأنواعها المختلفة");
                
                // Set the ID to 1 explicitly
                applicationDbContext.Categories.Add(defaultCategory);
                await applicationDbContext.SaveChangesAsync();
                
                // Update the ID to 1 after saving
                var category = await applicationDbContext.Categories.FirstAsync();
                if (category.Id != 1)
                {
                    // Delete and recreate with ID 1
                    applicationDbContext.Categories.Remove(category);
                    await applicationDbContext.SaveChangesAsync();
                    
                    var newCategory = new Category("البن والقهوة", "فئة البن والقهوة بأنواعها المختلفة");
                    applicationDbContext.Categories.Add(newCategory);
                    await applicationDbContext.SaveChangesAsync();
                    
                    // Use raw SQL to set ID to 1
                    await applicationDbContext.Database.ExecuteSqlRawAsync(
                        "UPDATE Categories SET Id = 1 WHERE Id = {0}", 
                        newCategory.Id);
                }
            }

            // Seed Products
            // if (!await applicationDbContext.Products.AnyAsync())
            // {
            //     List<Product> defaultProducts = [
            //         new Product(1, "بن برازيلي", "بن برازيلي عالي الجودة", "kg", 80000m, 100000m, 5m, true, true, 2000m, "111111111111"),
            //         new Product(1, "بن إثيوبي", "بن إثيوبي أرابيكا", "kg", 120000m, 150000m, 5m, true, true, 2500m, "222222222222"),
            //         new Product(1, "بن كولومبي", "بن كولومبي متوسط التحميص", "kg", 90000m, 120000m, 5m, true, true, 2200m, "333333333333"),
            //         new Product(1, "بن يمني", "بن يمني أصيل", "kg", 150000m, 200000m, 5m, true, true, 3000m, "444444444444"),
            //         new Product(1, "بن غواتيمالي", "بن غواتيمالي عالي الجودة", "kg", 110000m, 140000m, 5m, true, true, 2400m, "555555555555")
            //     ];

            //     await applicationDbContext.Products.AddRangeAsync(defaultProducts);
            //     await applicationDbContext.SaveChangesAsync();
            // }
        }
    }
}
