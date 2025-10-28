using System.Collections.Generic;
using MarketZone.Application.DTOs;
using MarketZone.Domain.Purchases.DTOs;
using MarketZone.Domain.Purchases.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace MarketZone.Application.Interfaces.Repositories
{
    public interface IPurchaseInvoiceRepository : IGenericRepository<PurchaseInvoice>
    {
        Task<PaginationResponseDto<PurchaseInvoiceDto>> GetPagedListAsync(int pageNumber, int pageSize, string invoiceNumber);
        Task<string> GetNextInvoiceNumberAsync(CancellationToken cancellationToken = default);
        Task<PurchaseInvoice> GetWithDetailsByIdAsync(long id, CancellationToken cancellationToken = default);
        Task<List<PurchaseInvoiceDto>> GetUnpaidInvoicesBySupplierAsync(long supplierId, CancellationToken cancellationToken = default);
        Task<bool> SupplierExistsAsync(long supplierId);
    }
}





