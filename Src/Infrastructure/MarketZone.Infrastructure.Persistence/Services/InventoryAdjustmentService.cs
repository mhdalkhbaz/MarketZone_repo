using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using MarketZone.Application.Interfaces.Services;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Domain.Inventory.Entities;
using MarketZone.Domain.Purchases.Entities;
using MarketZone.Domain.Cash.Enums;
using MarketZone.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System;

namespace MarketZone.Infrastructure.Persistence.Services
{
	public class InventoryAdjustmentService(ApplicationDbContext dbContext, IExchangeRateRepository exchangeRateRepository) : IInventoryAdjustmentService
	{
		public async Task AdjustOnPurchasePostedAsync(PurchaseInvoice invoice, CancellationToken cancellationToken = default)
		{
			var productIds = GetDistinctProductIds(invoice);
			var balances = await LoadBalancesAsync(productIds, cancellationToken);

			var totalsByProduct = GroupTotals(invoice);

			var requiresConversion = invoice.Currency.HasValue && invoice.Currency.Value == Currency.SY;
			decimal rate = 1m;
			if (requiresConversion)
			{
				var latest = await exchangeRateRepository.GetLatestActiveRateAsync(cancellationToken);
				if (latest == null || latest.Rate <= 0)
					throw new Exception("Active exchange rate is required to post a SYP purchase invoice.");
				rate = latest.Rate;
			}

			foreach (var kvp in totalsByProduct)
			{
				var productId = kvp.Key;
				var (quantity, value) = kvp.Value;
				var valueInUsd = requiresConversion ? decimal.Round(value / rate, 6) : value;
				await IncreaseReadyToSellAsync(balances, productId, quantity, valueInUsd, cancellationToken);
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


