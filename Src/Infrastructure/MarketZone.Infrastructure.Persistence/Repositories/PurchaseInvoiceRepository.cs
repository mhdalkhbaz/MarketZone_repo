using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MarketZone.Application.DTOs;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Domain.Cash.Enums;
using MarketZone.Domain.Purchases.DTOs;
using MarketZone.Domain.Purchases.Entities;
using MarketZone.Domain.Purchases.Enums;
using MarketZone.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;

namespace MarketZone.Infrastructure.Persistence.Repositories
{
	public class PurchaseInvoiceRepository(ApplicationDbContext dbContext, IMapper mapper) : GenericRepository<PurchaseInvoice>(dbContext), IPurchaseInvoiceRepository
	{
		public async Task<PaginationResponseDto<PurchaseInvoiceDto>> GetPagedListAsync(int pageNumber, int pageSize, string invoiceNumber)
		{
			var query = dbContext.Set<PurchaseInvoice>()
				.Include(x => x.Details)
				.OrderByDescending(p => p.InvoiceDate)
				.AsQueryable();

			if (!string.IsNullOrEmpty(invoiceNumber))
			{
				query = query.Where(p => p.InvoiceNumber.Contains(invoiceNumber));
			}

			return await Paged(
				query.ProjectTo<PurchaseInvoiceDto>(mapper.ConfigurationProvider),
				pageNumber,
				pageSize);
		}

		public async Task<string> GetNextInvoiceNumberAsync(CancellationToken cancellationToken = default)
		{
			var year = DateTime.UtcNow.Year;
			var prefix = $"SM-{year}-";
			var lastForYear = await dbContext.Set<PurchaseInvoice>()
				.Where(p => p.InvoiceNumber.StartsWith(prefix))
				.OrderByDescending(p => p.InvoiceNumber)
				.Select(p => p.InvoiceNumber)
				.FirstOrDefaultAsync(cancellationToken);

			int nextSeq = 1;
			if (!string.IsNullOrEmpty(lastForYear))
			{
				var parts = lastForYear.Split('-');
				if (parts.Length == 3 && int.TryParse(parts[2], out var seq))
				{
					nextSeq = seq + 1;
				}
			}

			return $"{prefix}{nextSeq.ToString("D5")}";
		}

		public async Task<PurchaseInvoice> GetWithDetailsByIdAsync(long id, CancellationToken cancellationToken = default)
		{
			return await dbContext.Set<PurchaseInvoice>()
				.Include(x => x.Details)
				.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
		}

		public async Task<List<PurchaseInvoiceDto>> GetUnpaidInvoicesBySupplierAsync(long supplierId, CancellationToken cancellationToken = default)
		{
			var unpaidInvoices = await dbContext.PurchaseInvoices
				.Where(pi => pi.SupplierId == supplierId && pi.Status == PurchaseInvoiceStatus.Posted)
				.GroupJoin(
					dbContext.Payments.Where(p => p.PaymentType == PaymentType.PurchasePayment && p.Status == MarketZone.Domain.Cash.Entities.PaymentStatus.Posted),
					pi => pi.Id,
					p => p.InvoiceId,
					(pi, payments) => new { Invoice = pi, Payments = payments }
				)
				.SelectMany(
					x => x.Payments.DefaultIfEmpty(),
					(x, payment) => new { x.Invoice, Payment = payment }
				)
				.GroupBy(x => x.Invoice.Id)
				.Select(g => new PurchaseInvoiceDto
				{
					Id = g.First().Invoice.Id,
					InvoiceNumber = g.First().Invoice.InvoiceNumber,
					SupplierId = g.First().Invoice.SupplierId,
					InvoiceDate = g.First().Invoice.InvoiceDate,
					TotalAmount = g.First().Invoice.TotalAmount,
					Discount = g.First().Invoice.Discount,
					Notes = g.First().Invoice.Notes,
					Currency = g.First().Invoice.Currency,
					Status = g.First().Invoice.Status,
					PaymentStatus = g.First().Invoice.PaymentStatus,
					CreatedDateTime = g.First().Invoice.Created,
					PaidAmount = g.Where(x => x.Payment != null).Sum(x => x.Payment.Amount),
					UnpaidAmount = g.First().Invoice.TotalAmount - g.First().Invoice.Discount - g.Where(x => x.Payment != null).Sum(x => x.Payment.Amount)
				})
				.Where(x => x.UnpaidAmount > 0) // فواتير لم يتم دفعها بالكامل (جزئياً أو غير مسددة)
				.OrderByDescending(x => x.InvoiceDate)
				.ToListAsync(cancellationToken);

			return unpaidInvoices;
		}

		public async Task<bool> SupplierExistsAsync(long supplierId)
		{
			return await dbContext.Suppliers.AnyAsync(s => s.Id == supplierId);
		}
	}
}



