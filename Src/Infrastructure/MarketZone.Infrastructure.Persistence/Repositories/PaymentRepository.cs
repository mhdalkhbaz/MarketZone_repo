using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MarketZone.Application.DTOs;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Parameters;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.DTOs;
using MarketZone.Domain.Cash.Entities;
using MarketZone.Domain.Cash.Enums;
using MarketZone.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace MarketZone.Infrastructure.Persistence.Repositories
{
	public class PaymentRepository(ApplicationDbContext dbContext, IMapper mapper) : GenericRepository<Payment>(dbContext), IPaymentRepository
	{
		public async Task<Payment> GetByIdAsync(long id, CancellationToken cancellationToken)
		{
			return await dbContext.Set<Payment>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
		}
		public async Task<decimal> GetPostedTotalForInvoiceAsync(long invoiceId, CancellationToken cancellationToken = default)
		{
			return await dbContext.Set<Payment>()
				.Where(p => p.InvoiceId == invoiceId && p.Status == PaymentStatus.Posted)
				.SumAsync(p => (decimal?)p.Amount, cancellationToken) ?? 0m;
		}

		public async Task<PagedResponse<PaymentDto>> GetPagedListAsync(PaymentFilter filter)
		{
			var query = dbContext.Set<Payment>().AsQueryable();

			// Apply filters using FilterBuilder pattern
			if (filter.InvoiceId.HasValue)
				query = query.Where(p => p.InvoiceId == filter.InvoiceId.Value);

			if (filter.CashRegisterId.HasValue)
				query = query.Where(p => p.CashRegisterId == filter.CashRegisterId.Value);

			if (filter.PaymentType.HasValue)
				query = query.Where(p => p.PaymentType == filter.PaymentType.Value);

			if (filter.FromDate.HasValue)
				query = query.Where(p => p.PaymentDate >= filter.FromDate.Value);

			if (filter.ToDate.HasValue)
				query = query.Where(p => p.PaymentDate <= filter.ToDate.Value);

			if (filter.IsIncome.HasValue && filter.IsIncome.Value)
				query = query.Where(p => p.PaymentType == PaymentType.SalesPayment);

			if (filter.IsExpense.HasValue && filter.IsExpense.Value)
				query = query.Where(p => p.PaymentType != PaymentType.SalesPayment);

			var totalCount = await query.CountAsync();
			var items = await query
				.OrderByDescending(p => p.PaymentDate)
				.Skip((filter.PageNumber - 1) * filter.PageSize)
				.Take(filter.PageSize)
				.ProjectTo<PaymentDto>(mapper.ConfigurationProvider)
				.ToListAsync();

			return PagedResponse<PaymentDto>.Ok(new PaginationResponseDto<PaymentDto>(items, totalCount, filter.PageNumber, filter.PageSize));
		}
	}
}


