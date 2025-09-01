using AutoMapper;
using MarketZone.Application.Features.Cash.CashRegisters.Commands.CreateCashRegister;
using MarketZone.Application.Features.Cash.CashRegisters.Commands.UpdateCashRegister;
using MarketZone.Application.Features.Cash.CashTransactions.Commands.CreateCashTransaction;
using MarketZone.Application.Features.Cash.Payments.Commands.CreatePayment;
using MarketZone.Application.Features.Cash.Payments.Commands.UpdatePayment;
using MarketZone.Domain.Cash.DTOs;
using MarketZone.Domain.Cash.Entities;

namespace MarketZone.Application.Features.Cash
{
	public class CashProfile : Profile
	{
		public CashProfile()
		{
			CreateMap<CashRegister, CashRegisterDto>();
			CreateMap<CreateCashRegisterCommand, CashRegister>()
				.ConstructUsing(s => new CashRegister(s.Name, s.OpeningBalance, s.OpeningBalanceDollar));
			CreateMap<UpdateCashRegisterCommand, CashRegister>()
				.ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

			CreateMap<CashTransaction, CashTransactionDto>();
			CreateMap<CreateCashTransactionCommand, CashTransaction>()
				.ConstructUsing(s => new CashTransaction(s.CashRegisterId, s.TransactionType, s.Amount, s.TransactionDate, s.ReferenceType, s.ReferenceId, s.Description));

			CreateMap<Payment, PaymentDto>();
			CreateMap<CreatePaymentCommand, Payment>()
				.ConstructUsing(s => new Payment(s.InvoiceId.Value, s.Amount, s.PaymentDate, s.Notes, s.ReceivedBy, s.IsConfirmed));
			CreateMap<UpdatePaymentCommand, Payment>()
				.ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
		}
	}
}


