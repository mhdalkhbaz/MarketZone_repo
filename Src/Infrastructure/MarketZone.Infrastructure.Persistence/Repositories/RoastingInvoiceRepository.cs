using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.DTOs;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Domain.Cash.Enums;
using MarketZone.Domain.Roasting.DTOs;
using MarketZone.Domain.Roasting.Entities;
using MarketZone.Domain.Roasting.Enums;
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
                .Include(x => x.Details)
                .ThenInclude(x => x.Receipts)
                .Include(x => x.Receipts)
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

        public async Task<List<RoastingInvoiceUnpaidDto>> GetUnpaidInvoicesByEmployeeAsync(long employeeId, System.Threading.CancellationToken cancellationToken = default)
        {
            var employeeCurrency = await dbContext.Employees
                .Where(e => e.Id == employeeId)
                .Select(e => e.Currency)
                .FirstOrDefaultAsync(cancellationToken);

            var unpaidInvoices = await dbContext.RoastingInvoices
                .Where(ri => ri.EmployeeId == employeeId && ri.Status == RoastingInvoiceStatus.Posted)
                .GroupJoin(
                    dbContext.Payments.Where(p => p.PaymentType == PaymentType.RoastingPayment && p.Status == MarketZone.Domain.Cash.Entities.PaymentStatus.Posted),
                    ri => ri.Id,
                    p => p.InvoiceId,
                    (ri, payments) => new { Invoice = ri, Payments = payments }
                )
                .SelectMany(
                    x => x.Payments.DefaultIfEmpty(),
                    (x, payment) => new { x.Invoice, Payment = payment }
                )
                .GroupBy(x => x.Invoice.Id)
                .Select(g => new RoastingInvoiceUnpaidDto
                {
                    Id = g.Key,
                    InvoiceNumber = g.First().Invoice.InvoiceNumber,
                    UnpaidAmount = g.First().Invoice.TotalAmount - g.Where(x => x.Payment != null).Sum(x => x.Payment.AmountInPaymentCurrency ?? x.Payment.Amount),
                    Currency = employeeCurrency ?? Currency.SY
                })
                .Where(x => x.UnpaidAmount > 0) // فواتير لم يتم دفعها بالكامل (جزئياً أو غير مسددة)
                .OrderByDescending(x => x.Id)
                .ToListAsync(cancellationToken);

            return unpaidInvoices;
        }

        public async Task<bool> EmployeeExistsAsync(long employeeId)
        {
            return await dbContext.Employees.AnyAsync(e => e.Id == employeeId);
        }

        public async Task<bool> HasRoastingInvoicesAsync(long employeeId, System.Threading.CancellationToken cancellationToken = default)
        {
            // التحقق من وجود فواتير تحميص مرتبطة بالموظف
            // نتحقق من الفواتير المرحلة (Posted) فقط، لأن الفواتير المسودة (Draft) يمكن تعديلها أو حذفها
            return await dbContext.RoastingInvoices
                .AnyAsync(ri => ri.EmployeeId == employeeId && ri.Status == RoastingInvoiceStatus.Posted, cancellationToken);
        }
    }
}
