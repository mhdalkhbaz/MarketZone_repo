using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using MarketZone.Application.Interfaces.Services;
using MarketZone.Domain.Inventory.Entities;
using MarketZone.Domain.Purchases.Entities;
using MarketZone.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace MarketZone.Infrastructure.Persistence.Services
{
	public class InventoryAdjustmentService(ApplicationDbContext dbContext) : IInventoryAdjustmentService
	{
		public async Task AdjustOnPurchasePostedAsync(PurchaseInvoice invoice, CancellationToken cancellationToken = default)
		{
			var productIds = GetDistinctProductIds(invoice);
			var balances = await LoadBalancesAsync(productIds, cancellationToken);

			var totalsByProduct = GroupTotals(invoice);

			foreach (var kvp in totalsByProduct)
			{
				var productId = kvp.Key;
				var (quantity, value) = kvp.Value;
				await IncreaseReadyToSellAsync(balances, productId, quantity, value, cancellationToken);
			}
		}

		private static List<long> GetDistinctProductIds(PurchaseInvoice invoice)
			=> invoice.Details
				.Select(d => d.ProductId)
				.Distinct()
				.ToList();

		private static Dictionary<long, (decimal qty, decimal value)> GroupTotals(PurchaseInvoice invoice)
			=> invoice.Details
				.GroupBy(d => d.ProductId)
				.ToDictionary(
					g => g.Key,
					g => (
						qty: g.Sum(x => x.Quantity),
						value: g.Sum(x => x.Quantity * x.UnitPrice)
					)
				);

		private async Task<Dictionary<long, ProductBalance>> LoadBalancesAsync(List<long> productIds, CancellationToken cancellationToken)
		{
			var list = await dbContext.Set<ProductBalance>()
				.Where(b => productIds.Contains(b.ProductId))
				.ToListAsync(cancellationToken);
			return list.ToDictionary(b => b.ProductId, b => b);
		}

		private async Task IncreaseReadyToSellAsync(Dictionary<long, ProductBalance> cache, long productId, decimal quantity, decimal valueToAdd, CancellationToken cancellationToken)
		{
			if (!cache.TryGetValue(productId, out var balance))
			{
				balance = new ProductBalance(productId, quantity, quantity, valueToAdd);
				await dbContext.Set<ProductBalance>().AddAsync(balance, cancellationToken);
				cache[productId] = balance;
				return;
			}

			balance.AdjustWithValue(quantity, quantity, valueToAdd);
		}
	}
}


