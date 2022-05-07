using AutoMapper;
using AutoMapper.QueryableExtensions;
using MarketZone.Application.DTOs;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Parameters;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.DTOs;
using MarketZone.Domain.Cash.Entities;
using MarketZone.Domain.Cash.Enums;
using MarketZone.Domain.Employees.Entities;
using MarketZone.Infrastructure.Persistence.Contexts;
using MarketZone.Infrastructure.Persistence.Migrations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MarketZone.Infrastructure.Persistence.Repositories
{
	public class PaymentRepository(ApplicationDbContext dbContext, IMapper mapper) : GenericRepository<Payment>(dbContext), IPaymentRepository
	{
		public async Task<Payment> GetByIdAsync(long id, CancellationToken cancellationToken)
		{
			return await dbContext.Set<Payment>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
		}
		public async Task<decimal> GetPostedTotalForInvoiceAsync(long invoiceId,InvoiceType? invoiceType = InvoiceType.SalesInvoice, CancellationToken cancellationToken = default)
		{
			return await dbContext.Set<Payment>()
				.Where(p => p.InvoiceId == invoiceId && p.Status == PaymentStatus.Posted 
												    	&& p.InvoiceType == invoiceType)
				.SumAsync(p => (decimal?)p.AmountInPaymentCurrency, cancellationToken) ?? 0m;
		}

		public async Task<PagedResponse<PaymentDto>> GetPagedListAsync(PaymentFilter filter)
		{
			var paymentQuery = dbContext.Set<Payment>().AsQueryable();

			// Handle SalaryPayment separately - merge from SalaryPayments table
			if (filter.PaymentType.HasValue && filter.PaymentType.Value == PaymentType.SalaryPayment)
			{
				// Query SalaryPayments instead
				var salaryPaymentQuery = dbContext.SalaryPayments
					.Include(sp => sp.Employee)
					.Include(sp => sp.CashRegister)
					.AsQueryable();

				if (filter.CashRegisterId.HasValue)
					salaryPaymentQuery = salaryPaymentQuery.Where(sp => sp.CashRegisterId == filter.CashRegisterId.Value);

				if (filter.FromDate.HasValue)
					salaryPaymentQuery = salaryPaymentQuery.Where(sp => sp.PaymentDate >= filter.FromDate.Value);

				if (filter.ToDate.HasValue)
					salaryPaymentQuery = salaryPaymentQuery.Where(sp => sp.PaymentDate <= filter.ToDate.Value);

				if (filter.IsExpense.HasValue && filter.IsExpense.Value)
				{
					// SalaryPayment is always expense, so include it
				}
				else if (filter.IsIncome.HasValue && filter.IsIncome.Value)
				{
					// SalaryPayment is not income, return empty
					salaryPaymentQuery = salaryPaymentQuery.Where(sp => false);
				}

				// Get total count
				var salaryTotalCount = await salaryPaymentQuery.CountAsync();

				// Get salary payments and convert to PaymentDto
				var salaryPayments = await salaryPaymentQuery
					.OrderByDescending(sp => sp.Id)
					.Skip((filter.PageNumber - 1) * filter.PageSize)
					.Take(filter.PageSize)
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

                foreach (var item in paymentDtos)
                {
                    switch (item.InvoiceType)
                    {
                        case InvoiceType.SalesInvoice:
						 item.InvoiceNumber = dbContext.SalesInvoices.FirstOrDefault(x => x.Id == item.InvoiceId).InvoiceNumber;
                            break;

                        case InvoiceType.PurchaseInvoice:
                            item.InvoiceNumber = dbContext.PurchaseInvoices.FirstOrDefault(x => x.Id == item.InvoiceId).InvoiceNumber;
                            break;

                        case InvoiceType.RoastingInvoice:
                            item.InvoiceNumber = dbContext.RoastingInvoices.FirstOrDefault(x => x.Id == item.InvoiceId).InvoiceNumber;
                            break;
                    }

                }
                return PagedResponse<PaymentDto>.Ok(new PaginationResponseDto<PaymentDto>(paymentDtos, salaryTotalCount, filter.PageNumber, filter.PageSize));
			}

			// Apply filters using FilterBuilder pattern
			if (filter.InvoiceId.HasValue)
				paymentQuery = paymentQuery.Where(p => p.InvoiceId == filter.InvoiceId.Value);

			if (filter.CashRegisterId.HasValue)
				paymentQuery = paymentQuery.Where(p => p.CashRegisterId == filter.CashRegisterId.Value);

			if (filter.PaymentType.HasValue)
				paymentQuery = paymentQuery.Where(p => p.PaymentType == filter.PaymentType.Value);

			if (filter.FromDate.HasValue)
				paymentQuery = paymentQuery.Where(p => p.PaymentDate >= filter.FromDate.Value);

			if (filter.ToDate.HasValue)
				paymentQuery = paymentQuery.Where(p => p.PaymentDate <= filter.ToDate.Value);

			if (filter.IsIncome.HasValue && filter.IsIncome.Value)
				paymentQuery = paymentQuery.Where(p => p.PaymentType == PaymentType.SalesPayment);

			if (filter.IsExpense.HasValue && filter.IsExpense.Value)
				paymentQuery = paymentQuery.Where(p => p.PaymentType != PaymentType.SalesPayment);

			var totalCount = await paymentQuery.CountAsync();
			var items = await paymentQuery
				.OrderByDescending(p => p.Id)
				.Skip((filter.PageNumber - 1) * filter.PageSize)
				.Take(filter.PageSize)
				.ProjectTo<PaymentDto>(mapper.ConfigurationProvider)
				.ToListAsync();

            foreach (var item in items)
            {
                switch (item.InvoiceType)
                {
                    case InvoiceType.SalesInvoice:
                        item.InvoiceNumber = dbContext.SalesInvoices.FirstOrDefault(x => x.Id == item.InvoiceId).InvoiceNumber;
                        break;

                    case InvoiceType.PurchaseInvoice:
                        item.InvoiceNumber = dbContext.PurchaseInvoices.FirstOrDefault(x => x.Id == item.InvoiceId).InvoiceNumber;
                        break;

                    case InvoiceType.RoastingInvoice:
                        item.InvoiceNumber = dbContext.RoastingInvoices.FirstOrDefault(x => x.Id == item.InvoiceId).InvoiceNumber;
                        break;
                }

            }
            return PagedResponse<PaymentDto>.Ok(new PaginationResponseDto<PaymentDto>(items, totalCount, filter.PageNumber, filter.PageSize));
		}

        public async Task<SelectList> GetUserForInviceByIdAsync(long invoiceId, InvoiceType invoiceType, CancellationToken cancellationToken)
        {
            var result = new SelectList();
            switch (invoiceType)
            {
                case InvoiceType.SalesInvoice:
                    result = dbContext.SalesInvoices.Where(x => x.Id == invoiceId).Select(x => new SelectList(x.CustomerId, x.Customer.Name)).FirstOrDefault();
                    break;

                case InvoiceType.PurchaseInvoice:
                    result = dbContext.PurchaseInvoices.Where(x => x.Id == invoiceId).Select(x => new SelectList(x.SupplierId, x.Supplier.Name)).FirstOrDefault();

                    break;

                case InvoiceType.RoastingInvoice:
                    result = dbContext.RoastingInvoices.Where(x => x.Id == invoiceId).Select(x => new SelectList(x.EmployeeId.Value,
						dbContext.Employees.FirstOrDefault(e=>e.Id == x.EmployeeId.Value).FirstName
						+" "+ dbContext.Employees.FirstOrDefault(e => e.Id == x.EmployeeId.Value).LastName)).FirstOrDefault();
                    break;
            }
			return result;
        }
    }
}


