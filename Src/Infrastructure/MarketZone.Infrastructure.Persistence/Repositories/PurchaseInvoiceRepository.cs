using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MarketZone.Application.DTOs;
using MarketZone.Application.Parameters;
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
	public async Task<PaginationResponseDto<PurchaseInvoiceDto>> GetPagedListAsync(PurchaseInvoiceFilter filter)
	{
		var query = dbContext.Set<PurchaseInvoice>()
			.Include(x => x.Details)
				.ThenInclude(d => d.Product)
			.Include(x => x.Supplier)
			.AsQueryable();

		// Apply filters using FilterBuilder pattern
		if (!string.IsNullOrEmpty(filter.InvoiceNumber))
		{
			query = query.Where(p => p.InvoiceNumber.Contains(filter.InvoiceNumber));
		}

		if (!string.IsNullOrEmpty(filter.SupplierName))
		{
			query = query.Where(p => p.Supplier != null && p.Supplier.Name.Contains(filter.SupplierName));
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

		query = query.OrderByDescending(p => p.Id);

		// استخدام Select مباشرة لإضافة اسم المورد
		var dtoQuery = query.Select(invoice => new PurchaseInvoiceDto
		{
			Id = invoice.Id,
			InvoiceNumber = invoice.InvoiceNumber,
			SupplierId = invoice.SupplierId,
			SupplierName = invoice.Supplier != null ? invoice.Supplier.Name : string.Empty,
			InvoiceDate = invoice.InvoiceDate,
			TotalAmount = invoice.TotalAmount,
			Discount = invoice.Discount,
			Notes = invoice.Notes,
			Currency = invoice.Currency,
			Status = invoice.Status,
			PaymentStatus = invoice.PaymentStatus,
			CreatedDateTime = invoice.Created,
			PaidAmount = 0, // سيتم حسابه من الدفعات
			UnpaidAmount = 0, // سيتم حسابه من الدفعات
			Details = invoice.Details.Select(d => new PurchaseInvoiceDetailDto
			{
				Id = d.Id,
				ProductId = d.ProductId,
				ProductName = d.Product != null ? d.Product.Name : string.Empty,
				Quantity = d.Quantity,
				UnitPrice = d.UnitPrice,
				TotalPrice = d.TotalPrice,
				Notes = d.Notes
			}).ToList()
		});

		return await Paged(dtoQuery, filter.PageNumber, filter.PageSize);
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
				.ThenInclude(d => d.Product)
			.Include(x => x.Supplier)
			.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
	}

		public async Task<List<PurchaseInvoiceDto>> GetUnpaidInvoicesBySupplierAsync(long supplierId, CancellationToken cancellationToken = default)
		{
			var supplierCurrency = await dbContext.Suppliers
				.Where(s => s.Id == supplierId)
				.Select(s => s.Currency)
				.FirstOrDefaultAsync(cancellationToken);

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
					PaidAmount = g.Where(x => x.Payment != null).Sum(x => x.Payment.AmountInPaymentCurrency ?? x.Payment.Amount),
					UnpaidAmount = g.First().Invoice.TotalAmount - g.First().Invoice.Discount - g.Where(x => x.Payment != null).Sum(x => x.Payment.AmountInPaymentCurrency ?? x.Payment.Amount)
				})
				.Where(x => x.UnpaidAmount > 0) // فواتير لم يتم دفعها بالكامل (جزئياً أو غير مسددة)
				.OrderByDescending(x => x.InvoiceDate)
				.ToListAsync(cancellationToken);

			unpaidInvoices.ForEach(i => i.Currency = supplierCurrency);

			return unpaidInvoices;
		}

		public async Task<List<PurchaseInvoiceUnpaidDto>> GetUnpaidInvoicesBySupplierSimpleAsync(long supplierId, CancellationToken cancellationToken = default)
		{
			var supplierCurrency = await dbContext.Suppliers
				.Where(s => s.Id == supplierId)
				.Select(s => s.Currency)
				.FirstOrDefaultAsync(cancellationToken);

			var invoices = await dbContext.PurchaseInvoices
				.Where(pi => pi.SupplierId == supplierId && pi.Status == PurchaseInvoiceStatus.Posted)
				.Select(pi => new
				{
					pi.Id,
					pi.InvoiceNumber,
					pi.TotalAmount,
					pi.Discount,
					pi.InvoiceDate,
					PaidAmount = dbContext.Payments
						.Where(p => p.PaymentType == PaymentType.PurchasePayment 
							&& p.Status == MarketZone.Domain.Cash.Entities.PaymentStatus.Posted
							&& p.InvoiceId == pi.Id)
						.Sum(p => (decimal?)(p.AmountInPaymentCurrency ?? p.Amount)) ?? 0
				})
				.ToListAsync(cancellationToken);

			var unpaidInvoices = invoices
				.Select(x => new PurchaseInvoiceUnpaidDto
				{
					Id = x.Id,
					InvoiceNumber = x.InvoiceNumber,
					TotalAmount = x.TotalAmount - x.Discount,
					UnpaidAmount = x.TotalAmount - x.Discount - x.PaidAmount,
					Currency = supplierCurrency ?? Currency.SY,
					InvoiceDate = x.InvoiceDate
				})
				.Where(x => x.UnpaidAmount > 0) // فواتير لم يتم دفعها بالكامل (جزئياً أو غير مسددة)
				.OrderByDescending(x => x.InvoiceDate)
				.ToList();

			return unpaidInvoices;
		}

		public async Task<bool> SupplierExistsAsync(long supplierId)
		{
			return await dbContext.Suppliers.AnyAsync(s => s.Id == supplierId);
		}
	}
}



