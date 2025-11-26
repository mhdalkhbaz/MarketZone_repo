using System;
using MarketZone.Domain.Cash.Enums;

namespace MarketZone.Application.Parameters
{
    public class CashTransactionFilter : PaginationRequestParameter
    {
        public long? CashRegisterId { get; set; }
        public TransactionType? TransactionType { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public ReferenceType? ReferenceType { get; set; }
        public long? ReferenceId { get; set; }
    }
}

