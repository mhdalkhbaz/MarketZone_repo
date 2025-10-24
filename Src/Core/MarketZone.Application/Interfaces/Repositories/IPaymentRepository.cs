using System;
using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.DTOs;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.DTOs;
using MarketZone.Domain.Cash.Entities;
using MarketZone.Domain.Cash.Enums;

namespace MarketZone.Application.Interfaces.Repositories
{
	public interface IPaymentRepository : IGenericRepository<Payment>
	{
		Task<PaginationResponseDto<PaymentDto>> GetPagedListAsync(int pageNumber, int pageSize, long? invoiceId, long? cashRegisterId);
		Task<PagedResponse<PaymentDto>> GetPagedListAsync(int pageNumber, int pageSize, long? invoiceId = null, long? cashRegisterId = null, PaymentType? paymentType = null, DateTime? fromDate = null, DateTime? toDate = null, bool? isIncome = null, bool? isExpense = null);
		Task<Payment> GetByIdAsync(long id, CancellationToken cancellationToken);
		Task<decimal> GetPostedTotalForInvoiceAsync(long invoiceId, CancellationToken cancellationToken = default);
	}
}


