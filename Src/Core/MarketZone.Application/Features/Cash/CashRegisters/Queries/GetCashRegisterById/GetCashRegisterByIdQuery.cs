using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.DTOs;

namespace MarketZone.Application.Features.Cash.CashRegisters.Queries.GetCashRegisterById
{
    public class GetCashRegisterByIdQuery : IRequest<BaseResult<CashRegisterDto>>
    {
        public long Id { get; set; }
    }
}
