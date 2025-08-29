using System.Threading.Tasks;
using MarketZone.Application.DTOs;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Domain.Roasting.Entities;

namespace MarketZone.Application.Interfaces.Repositories
{
    public interface IRoastingInvoiceRepository : IGenericRepository<RoastingInvoice>
    {
        Task<RoastingInvoice> GetWithDetailsByIdAsync(long id);
        Task<PaginationResponseDto<RoastingInvoice>> GetPagedListAsync(int pageNumber, int pageSize);
    }
}
