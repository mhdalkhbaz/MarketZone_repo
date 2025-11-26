using System;
using MarketZone.Domain.Cash.Enums;

namespace MarketZone.Application.Parameters
{
    public class PaymentFilter : PaginationRequestParameter
    {
        public long? InvoiceId { get; set; }
        public long? CashRegisterId { get; set; }
        public PaymentType? PaymentType { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public bool? IsIncome { get; set; }
        public bool? IsExpense { get; set; }
    }
}

