using System;
using MarketZone.Domain.Common;
using MarketZone.Domain.Cash.Enums;

namespace MarketZone.Domain.Cash.Entities
{
    public class ExchangeTransaction : AuditableBaseEntity
    {
        private ExchangeTransaction()
        {
        }

        public ExchangeTransaction(long cashRegisterId, ExchangeDirection direction, decimal fromAmount, 
                                 decimal toAmount, decimal exchangeRate, DateTime transactionDate, string notes)
        {
            CashRegisterId = cashRegisterId;
            Direction = direction;
            FromAmount = fromAmount;
            ToAmount = toAmount;
            ExchangeRate = exchangeRate;
            TransactionDate = transactionDate;
            Notes = notes;
        }

        public long CashRegisterId { get; private set; }
        public ExchangeDirection Direction { get; private set; }  // USD_To_SYP أو SYP_To_USD
        public decimal FromAmount { get; private set; }          // 100 USD أو 1,500,000 SYP
        public decimal ToAmount { get; private set; }            // 1,500,000 SYP أو 100 USD
        public decimal ExchangeRate { get; private set; }       // 15000
        public DateTime TransactionDate { get; private set; }
        public string Notes { get; private set; }

        // Helper properties
        public string FromCurrency => Direction == ExchangeDirection.USD_To_SYP ? "USD" : "SYP";
        public string ToCurrency => Direction == ExchangeDirection.USD_To_SYP ? "SYP" : "USD";
    }
}
