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
			if (invoice?.Details == null || invoice.Details.Count == 0)
				return;

			var productIds = GetDistinctProductIds(invoice);
			var products = await LoadProductsAsync(productIds, cancellationToken);
			var balances = await LoadBalancesAsync(productIds, cancellationToken);
			var unroasted = await LoadUnroastedAsync(productIds, cancellationToken);

			var totalByProduct = GroupTotalQuantities(invoice);

			foreach (var (productId, quantity) in totalByProduct)
			{
				if (!products.TryGetValue(productId, out var product))
					continue;

				if (product.NeedsRoasting)
				{
					await IncreaseUnroastedAsync(unroasted, productId, quantity, cancellationToken);
				}
				else
				{
					await IncreaseReadyToSellAsync(balances, productId, quantity, cancellationToken);
				}
			}
		}

		private static List<long> GetDistinctProductIds(PurchaseInvoice invoice)
			=> invoice.Details
				.Select(d => d.ProductId)
				.Distinct()
				.ToList();

		private static Dictionary<long, decimal> GroupTotalQuantities(PurchaseInvoice invoice)
			=> invoice.Details
				.GroupBy(d => d.ProductId)
				.ToDictionary(g => g.Key, g => g.Sum(x => x.Quantity));

		private async Task<Dictionary<long, Domain.Products.Entities.Product>> LoadProductsAsync(List<long> productIds, CancellationToken cancellationToken)
		{
			return await dbContext.Products
				.Where(p => productIds.Contains(p.Id))
				.ToDictionaryAsync(p => p.Id, cancellationToken);
		}

		private async Task<Dictionary<long, ProductBalance>> LoadBalancesAsync(List<long> productIds, CancellationToken cancellationToken)
		{
			var list = await dbContext.Set<ProductBalance>()
				.Where(b => productIds.Contains(b.ProductId))
				.ToListAsync(cancellationToken);
			return list.ToDictionary(b => b.ProductId, b => b);
		}

		private async Task<Dictionary<long, UnroastedProdcutBalance>> LoadUnroastedAsync(List<long> productIds, CancellationToken cancellationToken)
		{
			var list = await dbContext.Set<UnroastedProdcutBalance>()
				.Where(u => productIds.Contains(u.ProductId))
				.ToListAsync(cancellationToken);
			return list.ToDictionary(u => u.ProductId, u => u);
		}

		private async Task IncreaseUnroastedAsync(Dictionary<long, UnroastedProdcutBalance> cache, long productId, decimal quantity, CancellationToken cancellationToken)
		{
			if (!cache.TryGetValue(productId, out var row))
			{
				row = new UnroastedProdcutBalance(productId, quantity);
				await dbContext.Set<UnroastedProdcutBalance>().AddAsync(row, cancellationToken);
				cache[productId] = row;
				return;
			}

			row.Increase(quantity);
		}

		private async Task IncreaseReadyToSellAsync(Dictionary<long, ProductBalance> cache, long productId, decimal quantity, CancellationToken cancellationToken)
		{
			if (!cache.TryGetValue(productId, out var balance))
			{
				balance = new ProductBalance(productId, quantity, quantity);
				await dbContext.Set<ProductBalance>().AddAsync(balance, cancellationToken);
				cache[productId] = balance;
				return;
			}

			balance.Adjust(quantity, quantity);
		}
	}
}


