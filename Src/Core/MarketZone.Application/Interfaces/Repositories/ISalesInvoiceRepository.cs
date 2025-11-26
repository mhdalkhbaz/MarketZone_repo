using System.Collections.Generic;
using System.Threading.Tasks;
using MarketZone.Application.DTOs;
using MarketZone.Application.Parameters;
using MarketZone.Domain.Sales.DTOs;
using MarketZone.Domain.Sales.Entities;

namespace MarketZone.Application.Interfaces.Repositories
{
	public interface ISalesInvoiceRepository : IGenericRepository<SalesInvoice>
	{
		Task<PaginationResponseDto<SalesInvoiceDto>> GetPagedListAsync(SalesInvoiceFilter filter);
		Task<SalesInvoice> GetWithDetailsByIdAsync(long id, System.Threading.CancellationToken cancellationToken = default);
		Task<List<SalesInvoiceDto>> GetUnpaidInvoicesByCustomerAsync(long customerId, System.Threading.CancellationToken cancellationToken = default);
		Task<List<SalesInvoiceUnpaidDto>> GetUnpaidInvoicesByCustomerSimpleAsync(long customerId, System.Threading.CancellationToken cancellationToken = default);
		Task<bool> CustomerExistsAsync(long customerId);
		Task<bool> HasInvoicesForCustomerAsync(long customerId, System.Threading.CancellationToken cancellationToken = default);
		Task<List<SalesInvoice>> GetInvoicesByTripIdAsync(long tripId, System.Threading.CancellationToken cancellationToken = default);
	}
}



