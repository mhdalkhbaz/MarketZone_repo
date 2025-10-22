using AutoMapper;
using MarketZone.Application.Features.Cash.ExchangeRates.Commands.CreateExchangeRate;
using MarketZone.Domain.Cash.DTOs;
using MarketZone.Domain.Cash.Entities;

namespace MarketZone.Application.Features.Cash.ExchangeRates
{
    public class ExchangeRateProfile : Profile
    {
        public ExchangeRateProfile()
        {
            CreateMap<ExchangeRate, ExchangeRateDto>();
            CreateMap<CreateExchangeRateCommand, ExchangeRate>()
                .ConstructUsing(s => new ExchangeRate(s.Rate, s.EffectiveDate));
        }
    }
}
