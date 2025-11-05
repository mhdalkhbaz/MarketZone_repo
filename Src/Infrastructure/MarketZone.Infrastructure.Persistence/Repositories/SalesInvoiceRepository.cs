using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MarketZone.Application.DTOs;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Domain.Cash.Enums;
using MarketZone.Domain.Sales.DTOs;
using MarketZone.Domain.Sales.Entities;
using MarketZone.Domain.Sales.Enums;
using MarketZone.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace MarketZone.Infrastructure.Persistence.Repositories
{
	public class SalesInvoiceRepository(ApplicationDbContext dbContext, IMapper mapper) : GenericRepository<SalesInvoice>(dbContext), ISalesInvoiceRepository
	{
		public async Task<PaginationResponseDto<SalesInvoiceDto>> GetPagedListAsync(int pageNumber, int pageSize, string invoiceNumber)
		{
			var query = dbContext.Set<SalesInvoice>()
				.Include(x => x.Details)
				.OrderByDescending(p => p.InvoiceDate)
				.AsQueryable();

			if (!string.IsNullOrEmpty(invoiceNumber))
			{
				query = query.Where(p => p.InvoiceNumber.Contains(invoiceNumber));
			}

			return await Paged(
				query.ProjectTo<SalesInvoiceDto>(mapper.ConfigurationProvider),
				pageNumber,
				pageSize);
		}

		public async Task<SalesInvoice> GetWithDetailsByIdAsync(long id, System.Threading.CancellationToken cancellationToken = default)
		{
			return await dbContext.Set<SalesInvoice>()
				.Include(x => x.Details)
				.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
		}

		public async Task<List<SalesInvoiceDto>> GetUnpaidInvoicesByCustomerAsync(long customerId, System.Threading.CancellationToken cancellationToken = default)
		{
			var unpaidInvoices = await dbContext.SalesInvoices
				.Where(si => si.CustomerId == customerId && si.Status == SalesInvoiceStatus.Posted)
				.GroupJoin(
					dbContext.Payments.Where(p => p.PaymentType == PaymentType.SalesPayment && p.Status == MarketZone.Domain.Cash.Entities.PaymentStatus.Posted),
					si => si.Id,
					p => p.InvoiceId,
					(si, payments) => new { Invoice = si, Payments = payments }
				)
				.SelectMany(
					x => x.Payments.DefaultIfEmpty(),
					(x, payment) => new { x.Invoice, Payment = payment }
				)
				.GroupBy(x => x.Invoice.Id)
				.Select(g => new SalesInvoiceDto
				{
					Id = g.First().Invoice.Id,
					InvoiceNumber = g.First().Invoice.InvoiceNumber,
					CustomerId = g.First().Invoice.CustomerId,
					InvoiceDate = g.First().Invoice.InvoiceDate,
					TotalAmount = g.First().Invoice.TotalAmount,
					Discount = g.First().Invoice.Discount,
					PaymentMethod = g.First().Invoice.PaymentMethod,
					Notes = g.First().Invoice.Notes,
					Currency = g.First().Invoice.Currency,
					Status = g.First().Invoice.Status,
					Type = g.First().Invoice.Type,
					DistributionTripId = g.First().Invoice.DistributionTripId,
					CreatedDateTime = g.First().Invoice.Created,
					PaidAmount = g.Where(x => x.Payment != null).Sum(x => x.Payment.AmountInPaymentCurrency ?? x.Payment.Amount),
					UnpaidAmount = g.First().Invoice.TotalAmount - g.First().Invoice.Discount - g.Where(x => x.Payment != null).Sum(x => x.Payment.AmountInPaymentCurrency ?? x.Payment.Amount)
				})
				.Where(x => x.UnpaidAmount > 0) // فواتير لم يتم دفعها بالكامل (جزئياً أو غير مسددة)
				.OrderByDescending(x => x.InvoiceDate)
				.ToListAsync(cancellationToken);

			return unpaidInvoices;
		}

		public async Task<bool> CustomerExistsAsync(long customerId)
		{
			return await dbContext.Customers.AnyAsync(c => c.Id == customerId);
		}

		public async Task<bool> HasInvoicesForCustomerAsync(long customerId, System.Threading.CancellationToken cancellationToken = default)
		{
			return await dbContext.SalesInvoices.AnyAsync(si => si.CustomerId == customerId, cancellationToken);
		}

		public async Task<List<SalesInvoice>> GetInvoicesByTripIdAsync(long tripId, System.Threading.CancellationToken cancellationToken = default)
		{
			return await dbContext.SalesInvoices
				.Include(si => si.Details)
					.ThenInclude(d => d.Product)
				.Where(si => si.DistributionTripId == tripId)
				.ToListAsync(cancellationToken);
		}
	}
}



