using System;

namespace MarketZone.Application.Parameters
{
    public class ExchangeTransactionFilter : PaginationRequestParameter
    {
        public long? CashRegisterId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}

