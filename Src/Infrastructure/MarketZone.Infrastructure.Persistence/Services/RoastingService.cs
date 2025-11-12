using System;
using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Interfaces.Services;
using MarketZone.Domain.Inventory.Entities;
using MarketZone.Domain.Roasting.Entities;
using MarketZone.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace MarketZone.Infrastructure.Persistence.Services
{
    public class RoastingService(ApplicationDbContext dbContext) : IRoastingService
    {
		public async Task<long> RoastAsync(long rawProductId, long roastedProductId, decimal quantityKg, decimal roastPricePerKg, DateTime? roastDate, string notes, CancellationToken cancellationToken = default)
		{
			// التحقق من وجود الكمية في المنتج الخام (الني)
			var rawBalance = await dbContext.Set<ProductBalance>().FirstOrDefaultAsync(b => b.ProductId == rawProductId, cancellationToken);
			if (rawBalance == null || rawBalance.AvailableQty < quantityKg)
				throw new InvalidOperationException("Insufficient raw product quantity");

			// نقص الكمية من المنتج الخام
			rawBalance.Adjust(-quantityKg, -quantityKg);

			// إضافة الكمية إلى المنتج المحمص
			var roastedBalance = await dbContext.Set<ProductBalance>().FirstOrDefaultAsync(b => b.ProductId == roastedProductId, cancellationToken);
			if (roastedBalance == null)
			{
				// حساب القيمة للمنتج المحمص (تكلفة الخام + تكلفة التحميص)
				var rawValue = rawBalance.TotalValue / rawBalance.Qty * quantityKg; // القيمة النسبية للكمية المستخدمة
				var roastingCost = quantityKg * roastPricePerKg;
				var totalValue = rawValue + roastingCost;
				
				roastedBalance = new ProductBalance(roastedProductId, quantityKg, quantityKg, totalValue, 0m);
				await dbContext.Set<ProductBalance>().AddAsync(roastedBalance, cancellationToken);
			}
			else
			{
				// حساب القيمة للمنتج المحمص
				var rawValue = rawBalance.TotalValue / rawBalance.Qty * quantityKg;
				var roastingCost = quantityKg * roastPricePerKg;
				var totalValue = rawValue + roastingCost;
				
				roastedBalance.AdjustWithValue(quantityKg, quantityKg, totalValue);
			}

			// تسجيل في تاريخ المخزون
			await dbContext.Set<InventoryHistory>().AddAsync(new InventoryHistory(rawProductId, "Roast_Consume", null, -quantityKg, roastDate, notes));
			await dbContext.Set<InventoryHistory>().AddAsync(new InventoryHistory(roastedProductId, "Roast_Produce", null, quantityKg, roastDate, notes));

			// تسجيل عملية التحميص
			await dbContext.SaveChangesAsync(cancellationToken);
			return 2; // toDo
		}

        public Task<long> RoastAsync(long productId, decimal quantityKg, decimal roastPricePerKg, DateTime? roastDate, string notes, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}


