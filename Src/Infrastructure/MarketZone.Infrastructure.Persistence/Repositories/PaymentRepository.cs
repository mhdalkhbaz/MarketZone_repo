using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MarketZone.Application.DTOs;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Domain.Cash.DTOs;
using MarketZone.Domain.Cash.Entities;
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
	}
}


