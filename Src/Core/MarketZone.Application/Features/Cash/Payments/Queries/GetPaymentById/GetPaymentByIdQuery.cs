using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.DTOs;

namespace MarketZone.Application.Features.Cash.Payments.Queries.GetPaymentById
{
    public class GetPaymentByIdQuery : IRequest<BaseResult<PaymentDto>>
    {
        public long Id { get; set; }
    }
}

