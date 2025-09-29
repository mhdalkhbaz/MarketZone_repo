using System.Linq;
using System.Threading.Tasks;
using MarketZone.Application.DTOs;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Domain.Roasting.Entities;
using MarketZone.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace MarketZone.Infrastructure.Persistence.Repositories
{
    public class RoastingInvoiceRepository(ApplicationDbContext dbContext) : GenericRepository<RoastingInvoice>(dbContext), IRoastingInvoiceRepository
    {

        public async Task<RoastingInvoice> GetWithDetailsByIdAsync(long id)
        {
            var data = await dbContext.RoastingInvoices
                .Include(x => x.Details)
                .ThenInclude(x => x.RawProduct)
                .Include(x => x.Receipts)
                .ThenInclude(x => x.Detail)
                .Include(x => x.Payments)
                .FirstOrDefaultAsync(x => x.Id == id);
            return data;
        }

        public async Task<PaginationResponseDto<RoastingInvoice>> GetPagedListAsync(int pageNumber, int pageSize)
        {
            var query = dbContext.RoastingInvoices
                .Include(x => x.Details)
                .ThenInclude(x => x.RawProduct)
                .Include(x => x.Receipts)
                .Include(x => x.Payments)
                .OrderByDescending(x => x.Created);

            return await Paged(query, pageNumber, pageSize);
        }
    }
}
