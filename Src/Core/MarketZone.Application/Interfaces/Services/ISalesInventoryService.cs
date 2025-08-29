using System.Threading;
using System.Threading.Tasks;
using MarketZone.Domain.Sales.Entities;

namespace MarketZone.Application.Interfaces.Services
{
	public interface ISalesInventoryService
	{
		Task<bool> ReserveAvailableOnCreateAsync(SalesInvoice invoice, CancellationToken cancellationToken = default);
		Task ApplyOnPostAsync(SalesInvoice invoice, CancellationToken cancellationToken = default);
	}
}


