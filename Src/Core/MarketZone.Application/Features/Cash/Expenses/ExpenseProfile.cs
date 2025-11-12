using AutoMapper;
using MarketZone.Application.Features.Cash.Expenses.Commands.CreateExpense;
using MarketZone.Application.Features.Cash.Expenses.Commands.UpdateExpense;
using MarketZone.Domain.Cash.DTOs;
using MarketZone.Domain.Cash.Entities;

namespace MarketZone.Application.Features.Cash.Expenses
{
    public class ExpenseProfile : Profile
    {
        public ExpenseProfile()
        {
            CreateMap<CashTransaction, CashTransactionDto>();
            CreateMap<CreateExpenseCommand, CashTransaction>()
                .ConstructUsing(s => new CashTransaction(s.CashRegisterId, s.TransactionType, s.Amount, s.Currency, s.TransactionDate, s.ReferenceType, s.ReferenceId, s.Description));
            CreateMap<UpdateExpenseCommand, CashTransaction>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
