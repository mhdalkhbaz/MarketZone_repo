using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MarketZone.Application.DTOs;
using MarketZone.Application.Parameters;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Domain.Cash.Enums;
using MarketZone.Domain.Roasting.DTOs;
using MarketZone.Domain.Roasting.Entities;
using MarketZone.Domain.Roasting.Enums;
using MarketZone.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace MarketZone.Infrastructure.Persistence.Repositories
{
    public class RoastingInvoiceRepository(ApplicationDbContext dbContext, IMapper mapper) : GenericRepository<RoastingInvoice>(dbContext), IRoastingInvoiceRepository
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

        public async Task<PaginationResponseDto<RoastingInvoiceDto>> GetPagedListAsync(RoastingInvoiceFilter filter)
        {
            var query = dbContext.RoastingInvoices
                .Include(x => x.Details)
                .ThenInclude(x => x.RawProduct)
                .Include(x => x.Receipts)
                .Include(x => x.Payments)
                .AsQueryable();

            // Apply filters using FilterBuilder pattern
            if (!string.IsNullOrEmpty(filter.InvoiceNumber))
            {
                query = query.Where(p => p.InvoiceNumber.Contains(filter.InvoiceNumber));
            }

            if (!string.IsNullOrEmpty(filter.Name))
            {
                query = query.Where(p => p.InvoiceNumber.Contains(filter.Name));
            }

            if (!string.IsNullOrEmpty(filter.Notes))
            {
                query = query.Where(p => !string.IsNullOrEmpty(p.Notes) && p.Notes.Contains(filter.Notes));
            }

            if (filter.Status.HasValue)
            {
                query = query.Where(p => (int)p.Status == filter.Status.Value);
            }

            query = query.OrderByDescending(x => x.Created);

            var dtoQuery = query.ProjectTo<RoastingInvoiceDto>(mapper.ConfigurationProvider);

            return await Paged(dtoQuery, filter.PageNumber, filter.PageSize);
        }

        public async Task<List<RoastingInvoiceUnpaidDto>> GetUnpaidInvoicesByEmployeeAsync(long employeeId, System.Threading.CancellationToken cancellationToken = default)
        {
            var employeeCurrency = await dbContext.Employees
                .Where(e => e.Id == employeeId)
                .Select(e => e.Currency)
                .FirstOrDefaultAsync(cancellationToken);

            // جلب الفواتير مع Receipts و Payments
            var invoices = await dbContext.RoastingInvoices
                .Where(ri => ri.EmployeeId == employeeId && ri.Status == RoastingInvoiceStatus.Posted)
                .Include(ri => ri.Receipts)
                .ToListAsync(cancellationToken);

            var invoiceIds = invoices.Select(ri => ri.Id).ToList();

            // جلب جميع المدفوعات للفواتير
            var payments = await dbContext.Payments
                .Where(p => p.PaymentType == PaymentType.RoastingPayment 
                    && p.Status == MarketZone.Domain.Cash.Entities.PaymentStatus.Posted
                    && invoiceIds.Contains(p.InvoiceId.Value))
                .GroupBy(p => p.InvoiceId.Value)
                .Select(g => new { InvoiceId = g.Key, PaidAmount = g.Sum(p => p.AmountInPaymentCurrency ?? p.Amount) })
                .ToDictionaryAsync(x => x.InvoiceId, x => x.PaidAmount, cancellationToken);

            var unpaidInvoices = invoices
                .Select(ri =>
                {
                    // حساب TotalAmount من مجموع TotalRoastingCost من جميع Receipts
                    var totalAmount = ri.Receipts != null && ri.Receipts.Any()
                        ? ri.Receipts.Sum(r => r.TotalRoastingCost)
                        : ri.TotalAmount;

                    // حساب المبلغ المدفوع
                    var paidAmount = payments.TryGetValue(ri.Id, out var paid) ? paid : 0;

                    var unpaidAmount = totalAmount - paidAmount;

                    return new RoastingInvoiceUnpaidDto
                    {
                        Id = ri.Id,
                        InvoiceNumber = ri.InvoiceNumber,
                        UnpaidAmount = unpaidAmount,
                        Currency = employeeCurrency ?? Currency.SY
                    };
                })
                .Where(x => x.UnpaidAmount > 0) // فواتير لم يتم دفعها بالكامل (جزئياً أو غير مسددة)
                .OrderByDescending(x => x.Id)
                .ToList();

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
