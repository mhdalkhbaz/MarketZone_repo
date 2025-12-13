using System.Collections.Generic;
using System.Threading.Tasks;
using MarketZone.Application.DTOs;
using MarketZone.Application.Parameters;
using MarketZone.Domain.Roasting.DTOs;
using MarketZone.Domain.Roasting.Entities;

namespace MarketZone.Application.Interfaces.Repositories
{
    public interface IRoastingInvoiceRepository : IGenericRepository<RoastingInvoice>
    {
        Task<RoastingInvoice> GetWithDetailsByIdAsync(long id);
        Task<RoastingInvoice> GetById(long id);
        Task<PaginationResponseDto<RoastingInvoiceDto>> GetPagedListAsync(RoastingInvoiceFilter filter);
        Task<List<RoastingInvoiceUnpaidDto>> GetUnpaidInvoicesByEmployeeAsync(long employeeId, System.Threading.CancellationToken cancellationToken = default);
        Task<bool> EmployeeExistsAsync(long employeeId);
        Task<bool> HasRoastingInvoicesAsync(long employeeId, System.Threading.CancellationToken cancellationToken = default);
    }
}
