using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.DTOs;
using MarketZone.Domain.Cash.DTOs;
using MarketZone.Domain.Cash.Entities;

namespace MarketZone.Application.Interfaces.Repositories
{
	public interface IPaymentRepository : IGenericRepository<Payment>
	{
		Task<PaginationResponseDto<PaymentDto>> GetPagedListAsync(int pageNumber, int pageSize, long? invoiceId, long? cashRegisterId);
		Task<Payment> GetByIdAsync(long id, CancellationToken cancellationToken);
		Task<decimal> GetPostedTotalForInvoiceAsync(long invoiceId, CancellationToken cancellationToken = default);
	}
}


