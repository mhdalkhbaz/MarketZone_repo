using System;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Cash.ExchangeRates.Commands.CreateExchangeRate
{
    public class CreateExchangeRateCommand : IRequest<BaseResult<long>>
    {
        public decimal Rate { get; set; }                    // 15000 ليرة سورية
        public DateTime EffectiveDate { get; set; }         // تاريخ السعر
    }
}