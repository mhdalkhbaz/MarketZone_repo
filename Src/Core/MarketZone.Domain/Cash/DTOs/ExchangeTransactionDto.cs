using System;
using MarketZone.Domain.Cash.Enums;

namespace MarketZone.Domain.Cash.DTOs
{
    public class ExchangeTransactionDto
    {
        public long Id { get; set; }
        public long CashRegisterId { get; set; }
        public ExchangeDirection Direction { get; set; }
        public decimal FromAmount { get; set; }
        public decimal ToAmount { get; set; }
        public decimal ExchangeRate { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Notes { get; set; }
        
        // Helper properties
        public string FromCurrency => Direction == ExchangeDirection.USD_To_SYP ? "USD" : "SYP";
        public string ToCurrency => Direction == ExchangeDirection.USD_To_SYP ? "SYP" : "USD";
        public string DirectionText => Direction == ExchangeDirection.USD_To_SYP ? "دولار إلى سوري" : "سوري إلى دولار";
    }
}


