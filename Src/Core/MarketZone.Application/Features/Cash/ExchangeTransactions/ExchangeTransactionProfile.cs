using AutoMapper;
using MarketZone.Application.Features.Cash.ExchangeTransactions.Commands.CreateExchangeTransaction;
using MarketZone.Domain.Cash.DTOs;
using MarketZone.Domain.Cash.Entities;

namespace MarketZone.Application.Features.Cash.ExchangeTransactions
{
    public class ExchangeTransactionProfile : Profile
    {
        public ExchangeTransactionProfile()
        {
            CreateMap<ExchangeTransaction, ExchangeTransactionDto>();
            CreateMap<CreateExchangeTransactionCommand, ExchangeTransaction>()
                .ConstructUsing(s => new ExchangeTransaction(
                    s.CashRegisterId,
                    s.Direction,
                    s.FromAmount,
                    0, // ToAmount will be calculated in handler
                    s.ExchangeRate,
                    s.TransactionDate,
                    s.Notes
                ));
        }
    }
}

