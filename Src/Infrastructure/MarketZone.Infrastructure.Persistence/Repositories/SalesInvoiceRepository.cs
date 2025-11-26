using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MarketZone.Application.DTOs;
using MarketZone.Application.Parameters;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Domain.Cash.Enums;
using MarketZone.Domain.Sales.DTOs;
using MarketZone.Domain.Sales.Entities;
using MarketZone.Domain.Sales.Enums;
using MarketZone.Domain.Purchases.Enums;
using MarketZone.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace MarketZone.Infrastructure.Persistence.Repositories
{
	public class SalesInvoiceRepository(ApplicationDbContext dbContext, IMapper mapper) : GenericRepository<SalesInvoice>(dbContext), ISalesInvoiceRepository
	{
	public async Task<PaginationResponseDto<SalesInvoiceDto>> GetPagedListAsync(SalesInvoiceFilter filter)
	{
		var query = dbContext.Set<SalesInvoice>()
			.Include(x => x.Details)
				.ThenInclude(d => d.Product)
			.Include(x => x.Customer)
			.AsQueryable();

		// Apply filters using FilterBuilder pattern
		if (!string.IsNullOrEmpty(filter.InvoiceNumber))
		{
			query = query.Where(p => p.InvoiceNumber.Contains(filter.InvoiceNumber));
		}

		if (!string.IsNullOrEmpty(filter.CustomerName))
		{
			query = query.Where(p => p.Customer != null && p.Customer.Name.Contains(filter.CustomerName));
		}

		if (!string.IsNullOrEmpty(filter.Name))
		{
			query = query.Where(p => p.InvoiceNumber.Contains(filter.Name));
		}

		if (!string.IsNullOrEmpty(filter.Description))
		{
			query = query.Where(p => !string.IsNullOrEmpty(p.Notes) && p.Notes.Contains(filter.Description));
		}

		if (filter.Status.HasValue)
		{
			query = query.Where(p => (int)p.Status == filter.Status.Value);
		}

		if (filter.Type.HasValue)
		{
			query = query.Where(p => (int)p.Type == filter.Type.Value);
		}

		query = query.OrderByDescending(p => p.InvoiceDate);

		// استخدام Select مباشرة لإضافة اسم العميل و PaymentStatus
		var dtoQuery = query
			.GroupJoin(
				dbContext.Payments.Where(p => p.PaymentType == PaymentType.SalesPayment && p.Status == MarketZone.Domain.Cash.Entities.PaymentStatus.Posted),
				invoice => invoice.Id,
				payment => payment.InvoiceId,
				(invoice, payments) => new
				{
					Invoice = invoice,
					PaidAmount = payments.Where(p => p != null).Sum(p => (decimal?)(p.AmountInPaymentCurrency ?? p.Amount)) ?? 0
				})
			.Select(x => new SalesInvoiceDto
			{
				Id = x.Invoice.Id,
				InvoiceNumber = x.Invoice.InvoiceNumber,
				CustomerId = x.Invoice.CustomerId,
				CustomerName = x.Invoice.Customer != null ? x.Invoice.Customer.Name : string.Empty,
				InvoiceDate = x.Invoice.InvoiceDate,
				TotalAmount = x.Invoice.TotalAmount,
				Discount = x.Invoice.Discount,
				PaymentMethod = x.Invoice.PaymentMethod,
				Notes = x.Invoice.Notes,
				Currency = x.Invoice.Currency,
				Status = x.Invoice.Status,
				Type = x.Invoice.Type,
				DistributionTripId = x.Invoice.DistributionTripId,
				CreatedDateTime = x.Invoice.Created,
				PaidAmount = x.PaidAmount,
				UnpaidAmount = x.Invoice.TotalAmount - x.Invoice.Discount - x.PaidAmount,
				PaymentStatus = x.PaidAmount >= (x.Invoice.TotalAmount - x.Invoice.Discount) 
					? PurchasePaymentStatus.CompletePayment 
					: x.PaidAmount > 0 
						? PurchasePaymentStatus.PartialPayment 
						: PurchasePaymentStatus.InProgress,
				Details = x.Invoice.Details.Select(d => new SalesInvoiceDetailDto
				{
					Id = d.Id,
					ProductId = d.ProductId,
					ProductName = d.Product != null ? d.Product.Name : string.Empty,
					Quantity = d.Quantity,
					UnitPrice = d.UnitPrice,
					SubTotal = d.SubTotal,
					Notes = d.Notes
				}).ToList()
			});

		return await Paged(dtoQuery, filter.PageNumber, filter.PageSize);
	}

	public async Task<SalesInvoice> GetWithDetailsByIdAsync(long id, System.Threading.CancellationToken cancellationToken = default)
	{
		return await dbContext.Set<SalesInvoice>()
			.Include(x => x.Details)
				.ThenInclude(d => d.Product)
			.Include(x => x.Customer)
			.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
	}

		public async Task<List<SalesInvoiceDto>> GetUnpaidInvoicesByCustomerAsync(long customerId, System.Threading.CancellationToken cancellationToken = default)
		{
			var unpaidInvoices = await dbContext.SalesInvoices
				.Include(si => si.Details)
					.ThenInclude(d => d.Product)
				.Include(si => si.Customer)
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
				.Select(g => new
				{
					Invoice = g.First().Invoice,
					PaidAmount = g.Where(x => x.Payment != null).Sum(x => x.Payment.AmountInPaymentCurrency ?? x.Payment.Amount)
				})
				.Select(x => new SalesInvoiceDto
				{
					Id = x.Invoice.Id,
					InvoiceNumber = x.Invoice.InvoiceNumber,
					CustomerId = x.Invoice.CustomerId,
					CustomerName = x.Invoice.Customer != null ? x.Invoice.Customer.Name : string.Empty,
					InvoiceDate = x.Invoice.InvoiceDate,
					TotalAmount = x.Invoice.TotalAmount,
					Discount = x.Invoice.Discount,
					PaymentMethod = x.Invoice.PaymentMethod,
					Notes = x.Invoice.Notes,
					Currency = x.Invoice.Currency ?? Currency.SY,
					Status = x.Invoice.Status,
					Type = x.Invoice.Type,
					DistributionTripId = x.Invoice.DistributionTripId,
					CreatedDateTime = x.Invoice.Created,
					PaidAmount = x.PaidAmount,
					UnpaidAmount = x.Invoice.TotalAmount - x.Invoice.Discount - x.PaidAmount,
					PaymentStatus = x.PaidAmount >= (x.Invoice.TotalAmount - x.Invoice.Discount) 
						? PurchasePaymentStatus.CompletePayment 
						: x.PaidAmount > 0 
							? PurchasePaymentStatus.PartialPayment 
							: PurchasePaymentStatus.InProgress,
					Details = x.Invoice.Details.Select(d => new SalesInvoiceDetailDto
					{
						Id = d.Id,
						ProductId = d.ProductId,
						ProductName = d.Product != null ? d.Product.Name : string.Empty,
						Quantity = d.Quantity,
						UnitPrice = d.UnitPrice,
						SubTotal = d.SubTotal,
						Notes = d.Notes
					}).ToList()
				})
				.Where(x => x.UnpaidAmount > 0) // فواتير لم يتم دفعها بالكامل (جزئياً أو غير مسددة)
				.OrderByDescending(x => x.InvoiceDate)
				.ToListAsync(cancellationToken);

		return unpaidInvoices;
	}

		public async Task<List<SalesInvoiceUnpaidDto>> GetUnpaidInvoicesByCustomerSimpleAsync(long customerId, System.Threading.CancellationToken cancellationToken = default)
		{
			var invoices = await dbContext.SalesInvoices
				.Where(si => si.CustomerId == customerId && si.Status == SalesInvoiceStatus.Posted)
				.Select(si => new
				{
					si.Id,
					si.InvoiceNumber,
					si.TotalAmount,
					si.Discount,
					si.Currency,
					si.InvoiceDate,
					PaidAmount = dbContext.Payments
						.Where(p => p.PaymentType == PaymentType.SalesPayment 
							&& p.Status == MarketZone.Domain.Cash.Entities.PaymentStatus.Posted
							&& p.InvoiceId == si.Id)
						.Sum(p => (decimal?)(p.AmountInPaymentCurrency ?? p.Amount)) ?? 0
				})
				.ToListAsync(cancellationToken);

			var unpaidInvoices = invoices
				.Select(x => new SalesInvoiceUnpaidDto
				{
					Id = x.Id,
					InvoiceNumber = x.InvoiceNumber,
					TotalAmount = x.TotalAmount - x.Discount,
					UnpaidAmount = x.TotalAmount - x.Discount - x.PaidAmount,
					Currency = x.Currency ?? Currency.SY,
					InvoiceDate = x.InvoiceDate
				})
				.Where(x => x.UnpaidAmount > 0) // فواتير لم يتم دفعها بالكامل (جزئياً أو غير مسددة)
				.OrderByDescending(x => x.InvoiceDate)
				.ToList();

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



