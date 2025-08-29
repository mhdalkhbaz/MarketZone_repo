using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MarketZone.Domain.Inventory.Entities;
using MarketZone.Domain.Sales.Entities;
using MarketZone.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using MarketZone.Application.Interfaces.Services;

namespace MarketZone.Infrastructure.Persistence.Services
{
	public class SalesInventoryService : ISalesInventoryService
	{
		private readonly ApplicationDbContext dbContext;
		public SalesInventoryService(ApplicationDbContext dbContext)
		{
			this.dbContext = dbContext;
		}

		public async Task<bool> ReserveAvailableOnCreateAsync(SalesInvoice invoice, CancellationToken cancellationToken = default)
		{
			var productIds = invoice.Details.Select(d => d.ProductId).Distinct().ToList();
			var balances = await dbContext.Set<ProductBalance>().Where(b => productIds.Contains(b.ProductId)).ToDictionaryAsync(b => b.ProductId, cancellationToken);

			foreach (var d in invoice.Details)
			{
				if (!balances.TryGetValue(d.ProductId, out var bal) || bal.AvailableQty < d.Quantity)
					return false;
			}
			foreach (var d in invoice.Details)
			{
				var bal = balances[d.ProductId];
				bal.Adjust(0, -d.Quantity);
				await dbContext.Set<InventoryHistory>().AddAsync(new InventoryHistory(d.ProductId, "ReserveSale", invoice.Id, d.Quantity, invoice.InvoiceDate, invoice.InvoiceNumber));
			}
			return true;
		}

		public async Task ApplyOnPostAsync(SalesInvoice invoice, CancellationToken cancellationToken = default)
		{
			var productIds = invoice.Details.Select(d => d.ProductId).Distinct().ToList();
			var balances = await dbContext.Set<ProductBalance>().Where(b => productIds.Contains(b.ProductId)).ToDictionaryAsync(b => b.ProductId, cancellationToken);
			foreach (var d in invoice.Details)
			{
				var bal = balances[d.ProductId];
				bal.Adjust(-d.Quantity, 0);
				await dbContext.Set<InventoryHistory>().AddAsync(new InventoryHistory(d.ProductId, "Sale", invoice.Id, d.Quantity, invoice.InvoiceDate, invoice.InvoiceNumber));
			}
		}
	}
}


