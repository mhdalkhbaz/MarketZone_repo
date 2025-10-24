using AutoMapper;
using MarketZone.Application.Features.Cash.Payments.Commands.CreatePayment;
using MarketZone.Domain.Cash.DTOs;
using MarketZone.Domain.Cash.Entities;
using MarketZone.Domain.Cash.Enums;

namespace MarketZone.Application.Features.Cash.Payments
{
    public class PaymentProfile : Profile
    {
        public PaymentProfile()
        {
            CreateMap<Payment, PaymentDto>();
            CreateMap<CreatePaymentCommand, Payment>()
                .ConstructUsing(s => RequiresInvoice(s.PaymentType) 
                    ? new Payment(s.PaymentType, s.InvoiceId.Value, s.InvoiceType.Value, s.Amount, s.PaymentDate, s.Currency, s.PaymentCurrency, s.ExchangeRate, s.Notes, s.ReceivedBy, s.IsConfirmed)
                    : new Payment(s.PaymentType, s.Amount, s.PaymentDate ?? System.DateTime.UtcNow, s.Description, s.PaidBy, s.IsConfirmed));
        }

        private static bool RequiresInvoice(PaymentType type)
        {
            return type == PaymentType.SalesPayment ||
                   type == PaymentType.PurchasePayment ||
                   type == PaymentType.RoastingPayment;
        }
    }
}
