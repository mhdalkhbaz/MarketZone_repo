using System;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.Enums;

namespace MarketZone.Application.Features.Cash.ExchangeTransactions.Commands.CreateExchangeTransaction
{
    public class CreateExchangeTransactionCommand : IRequest<BaseResult<long>>
    {
        public long CashRegisterId { get; set; }
        public ExchangeDirection Direction { get; set; }     // USD_To_SYP أو SYP_To_USD
        public decimal FromAmount { get; set; }              // المبلغ المراد تحويله
        public decimal ExchangeRate { get; set; }           // سعر الصرف
        public DateTime TransactionDate { get; set; }
        public string Notes { get; set; }
    }
}
