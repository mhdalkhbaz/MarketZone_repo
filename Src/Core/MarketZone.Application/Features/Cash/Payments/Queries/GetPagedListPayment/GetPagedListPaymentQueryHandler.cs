using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.DTOs;

namespace MarketZone.Application.Features.Cash.Payments.Queries.GetPagedListPayment
{
    public class GetPagedListPaymentQueryHandler(IPaymentRepository repository) : IRequestHandler<GetPagedListPaymentQuery, PagedResponse<PaymentDto>>
    {
        public async Task<PagedResponse<PaymentDto>> Handle(GetPagedListPaymentQuery request, CancellationToken cancellationToken)
        {
            return await repository.GetPagedListAsync(request);
        }
    }
}
