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
using MarketZone.Domain.Employees.Entities;

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
			var paymentQuery = dbContext.Set<Payment>().AsQueryable();

			// Handle SalaryPayment separately - merge from SalaryPayments table
			if (paymentType.HasValue && paymentType.Value == PaymentType.SalaryPayment)
			{
				// Query SalaryPayments instead
				var salaryPaymentQuery = dbContext.SalaryPayments
					.Include(sp => sp.Employee)
					.Include(sp => sp.CashRegister)
					.AsQueryable();

				if (cashRegisterId.HasValue)
					salaryPaymentQuery = salaryPaymentQuery.Where(sp => sp.CashRegisterId == cashRegisterId.Value);

				if (fromDate.HasValue)
					salaryPaymentQuery = salaryPaymentQuery.Where(sp => sp.PaymentDate >= fromDate.Value);

				if (toDate.HasValue)
					salaryPaymentQuery = salaryPaymentQuery.Where(sp => sp.PaymentDate <= toDate.Value);

				if (isExpense.HasValue && isExpense.Value)
				{
					// SalaryPayment is always expense, so include it
				}
				else if (isIncome.HasValue && isIncome.Value)
				{
					// SalaryPayment is not income, return empty
					salaryPaymentQuery = salaryPaymentQuery.Where(sp => false);
				}

				// Get total count
				var totalCount = await salaryPaymentQuery.CountAsync();

				// Get salary payments and convert to PaymentDto
				var salaryPayments = await salaryPaymentQuery
					.OrderByDescending(sp => sp.PaymentDate)
					.Skip((pageNumber - 1) * pageSize)
					.Take(pageSize)
					.ToListAsync();

				// Get all salary payment IDs for efficient CashTransaction lookup
				var salaryPaymentIds = salaryPayments.Select(sp => sp.Id).ToList();
				var cashTransactions = await dbContext.Set<CashTransaction>()
					.Where(ct => ct.ReferenceType == ReferenceType.Salaries && salaryPaymentIds.Contains(ct.ReferenceId.Value))
					.ToListAsync();
				
				// Convert SalaryPayment to PaymentDto
				var paymentDtos = new List<PaymentDto>();
				foreach (var sp in salaryPayments)
				{
					// Get Currency from CashTransaction if exists, otherwise from Employee or default
					var currency = Currency.SY;
					var cashTransaction = cashTransactions.FirstOrDefault(ct => ct.ReferenceId == sp.Id);
					
					if (cashTransaction != null)
					{
						currency = cashTransaction.Currency;
					}
					else if (sp.Employee?.Currency.HasValue == true)
					{
						currency = sp.Employee.Currency.Value;
					}

					var paymentDto = new PaymentDto
					{
						Id = sp.Id,
						CashRegisterId = sp.CashRegisterId,
						PaymentType = PaymentType.SalaryPayment,
						InvoiceId = null,
						InvoiceType = null,
						PaymentDate = sp.PaymentDate,
						Amount = sp.Amount,
						Currency = currency,
						PaymentCurrency = currency,
						ExchangeRate = null,
						AmountInPaymentCurrency = sp.Amount,
						Notes = sp.Notes,
						Description = $"Salary payment for {sp.Employee?.FirstName} {sp.Employee?.LastName} - {sp.Year}/{sp.Month}",
						ReceivedBy = null,
						PaidBy = sp.Employee != null ? $"{sp.Employee.FirstName} {sp.Employee.LastName}" : null,
						IsConfirmed = true,
						Status = PaymentStatus.Posted, // SalaryPayment is always posted
						IsIncome = false,
						IsExpense = true
					};
					paymentDtos.Add(paymentDto);
				}

				return PagedResponse<PaymentDto>.Ok(new PaginationResponseDto<PaymentDto>(paymentDtos, totalCount, pageNumber, pageSize));
			}

			// Handle regular Payment types
			if (invoiceId.HasValue)
				paymentQuery = paymentQuery.Where(p => p.InvoiceId == invoiceId.Value);

			if (cashRegisterId.HasValue)
				paymentQuery = paymentQuery.Where(p => p.CashRegisterId == cashRegisterId.Value);

			if (paymentType.HasValue)
				paymentQuery = paymentQuery.Where(p => p.PaymentType == paymentType.Value);

			if (fromDate.HasValue)
				paymentQuery = paymentQuery.Where(p => p.PaymentDate >= fromDate.Value);

			if (toDate.HasValue)
				paymentQuery = paymentQuery.Where(p => p.PaymentDate <= toDate.Value);

			if (isIncome.HasValue && isIncome.Value)
				paymentQuery = paymentQuery.Where(p => p.PaymentType == PaymentType.SalesPayment);

			if (isExpense.HasValue && isExpense.Value)
				paymentQuery = paymentQuery.Where(p => p.PaymentType != PaymentType.SalesPayment);

			var totalCount = await paymentQuery.CountAsync();
			var items = await paymentQuery
				.OrderByDescending(p => p.PaymentDate)
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
				.ProjectTo<PaymentDto>(mapper.ConfigurationProvider)
				.ToListAsync();

			return PagedResponse<PaymentDto>.Ok(new PaginationResponseDto<PaymentDto>(items, totalCount, pageNumber, pageSize));
		}
	}
}


