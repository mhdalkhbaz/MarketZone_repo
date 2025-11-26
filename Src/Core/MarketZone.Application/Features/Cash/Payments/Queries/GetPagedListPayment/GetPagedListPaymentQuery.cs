using MarketZone.Application.Interfaces;
using MarketZone.Application.Parameters;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.DTOs;

namespace MarketZone.Application.Features.Cash.Payments.Queries.GetPagedListPayment
{
    public class GetPagedListPaymentQuery : PaymentFilter, IRequest<PagedResponse<PaymentDto>>
    {
    }
}
