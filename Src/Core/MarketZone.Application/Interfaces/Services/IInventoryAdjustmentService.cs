using System.Threading;
using System.Threading.Tasks;
using MarketZone.Domain.Purchases.Entities;

namespace MarketZone.Application.Interfaces.Services
{
	public interface IInventoryAdjustmentService
	{
		Task AdjustOnPurchasePostedAsync(PurchaseInvoice invoice, CancellationToken cancellationToken = default);
	}
}


