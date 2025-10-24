using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.DTOs;
using MarketZone.Domain.Cash.Enums;
using System;

namespace MarketZone.Application.Features.Cash.Payments.Queries.GetPagedListPayment
{
    public class GetPagedListPaymentQuery : IRequest<PagedResponse<PaymentDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public long? InvoiceId { get; set; }
        public long? CashRegisterId { get; set; }
        public PaymentType? PaymentType { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public bool? IsIncome { get; set; }
        public bool? IsExpense { get; set; }
    }
}
