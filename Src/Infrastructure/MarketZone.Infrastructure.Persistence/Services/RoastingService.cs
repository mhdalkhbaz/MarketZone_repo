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
		public async Task<long> RoastAsync(long productId, decimal quantityKg, decimal roastPricePerKg, DateTime? roastDate, string notes, CancellationToken cancellationToken = default)
		{
			var unroasted = await dbContext.Set<UnroastedProdcutBalance>().FirstOrDefaultAsync(u => u.ProductId == productId, cancellationToken);
			if (unroasted == null || unroasted.Qty < quantityKg)
				throw new InvalidOperationException("Insufficient unroasted quantity");

			unroasted.Decrease(quantityKg);

			var balance = await dbContext.Set<ProductBalance>().FirstOrDefaultAsync(b => b.ProductId == productId, cancellationToken);
			if (balance == null)
			{
				balance = new ProductBalance(productId, quantityKg, quantityKg);
				await dbContext.Set<ProductBalance>().AddAsync(balance, cancellationToken);
			}
			else
			{
				balance.Adjust(quantityKg, quantityKg);
			}

			await dbContext.Set<InventoryHistory>().AddAsync(new InventoryHistory(productId, "Roast", null, quantityKg, roastDate, notes));

			var op = new RoastingOperation(productId, quantityKg, roastPricePerKg, roastDate, notes);
			await dbContext.Set<RoastingOperation>().AddAsync(op, cancellationToken);
			await dbContext.SaveChangesAsync(cancellationToken);
			return op.Id;
		}
	}
}


