using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MarketZone.Application.DTOs;
using MarketZone.Application.Interfaces.Repositories;
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
		public async Task<PaginationResponseDto<PaymentDto>> GetPagedListAsync(int pageNumber, int pageSize, long? invoiceId, long? cashRegisterId)
		{
			var query = dbContext.Set<Payment>().OrderByDescending(p => p.PaymentDate).AsQueryable();
			if (invoiceId.HasValue) query = query.Where(p => p.InvoiceId == invoiceId.Value);
			if (cashRegisterId.HasValue) query = query.Where(p => p.CashRegisterId == cashRegisterId.Value);
			return await Paged(query.ProjectTo<PaymentDto>(mapper.ConfigurationProvider), pageNumber, pageSize);
		}

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

		public async Task<PagedResponse<PaymentDto>> GetPagedListAsync(int pageNumber, int pageSize, long? invoiceId = null, long? cashRegisterId = null, PaymentType? paymentType = null, DateTime? fromDate = null, DateTime? toDate = null, bool? isIncome = null, bool? isExpense = null)
		{
			var query = dbContext.Set<Payment>().AsQueryable();

			if (invoiceId.HasValue)
				query = query.Where(p => p.InvoiceId == invoiceId.Value);

			if (cashRegisterId.HasValue)
				query = query.Where(p => p.CashRegisterId == cashRegisterId.Value);

			if (paymentType.HasValue)
				query = query.Where(p => p.PaymentType == paymentType.Value);

			if (fromDate.HasValue)
				query = query.Where(p => p.PaymentDate >= fromDate.Value);

			if (toDate.HasValue)
				query = query.Where(p => p.PaymentDate <= toDate.Value);

			if (isIncome.HasValue && isIncome.Value)
				query = query.Where(p => p.PaymentType == PaymentType.SalesPayment);

			if (isExpense.HasValue && isExpense.Value)
				query = query.Where(p => p.PaymentType != PaymentType.SalesPayment);

			var totalCount = await query.CountAsync();
			var items = await query
				.OrderByDescending(p => p.PaymentDate)
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
				.ProjectTo<PaymentDto>(mapper.ConfigurationProvider)
				.ToListAsync();

			return PagedResponse<PaymentDto>.Ok(new PaginationResponseDto<PaymentDto> (items,totalCount,pageNumber,pageSize));
		}
	}
}


