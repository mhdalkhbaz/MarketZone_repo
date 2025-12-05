using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MarketZone.Application.DTOs;
using MarketZone.Application.Helpers;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.Entities;
using MarketZone.Domain.Cash.Enums;

namespace MarketZone.Application.Features.Cash.Expenses.Commands.CreateExpense
{
    public class CreateExpenseCommandHandler(
        ICashTransactionRepository repository,
        ICashRegisterRepository cashRegisterRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ITranslator translator) : IRequestHandler<CreateExpenseCommand, BaseResult<long>>
    {
        public async Task<BaseResult<long>> Handle(CreateExpenseCommand request, CancellationToken cancellationToken)
        {
            var entity = mapper.Map<CashTransaction>(request);
            await repository.AddAsync(entity);

            var cashRegister = await cashRegisterRepository.GetByIdAsync(request.CashRegisterId);
            if (cashRegister == null)
            {
                var message = translator.GetString(TranslatorMessages.CashRegisterMessages.CashRegister_NotFound_with_id(request.CashRegisterId));
                return new Error(ErrorCode.NotFound, message, nameof(request.CashRegisterId));
            }

            // التحقق من الرصيد عند المصروفات قبل التنفيذ
            if (request.TransactionType == TransactionType.Expense)
            {
                if (request.Currency == Domain.Cash.Enums.Currency.SY)
                {
                    if (cashRegister.CurrentBalance < request.Amount)
                    {
                        var message = translator.GetString(new TranslatorMessageDto(
                            "Insufficient_Balance_In_Cash_Register",
                            [cashRegister.CurrentBalance.ToString(), request.Amount.ToString()]));
                        return new Error(ErrorCode.FieldDataInvalid, message, nameof(request.Amount));
                    }
                }
                else if (request.Currency == Domain.Cash.Enums.Currency.Dollar)
                {
                    if (cashRegister.CurrentBalanceDollar < request.Amount)
                    {
                        var message = translator.GetString(new TranslatorMessageDto(
                            "Insufficient_Dollar_Balance_In_Cash_Register",
                            [cashRegister.CurrentBalanceDollar.ToString(), request.Amount.ToString()]));
                        return new Error(ErrorCode.FieldDataInvalid, message, nameof(request.Amount));
                    }
                }
            }

            // تنفيذ التعديل بعد اجتياز التحقق
            {
                // Income = 0: يضيف على الصندوق
                // Expense = 1: يخرج من الصندوق
                if (request.TransactionType == TransactionType.Income)
            {
                    // إضافة على الصندوق حسب العملة
                    if (request.Currency == Domain.Cash.Enums.Currency.SY)
                        cashRegister.Adjust(request.Amount, null);
                    else
                        cashRegister.Adjust(0, request.Amount);
                }
                else // Expense
                {
                    // خروج من الصندوق حسب العملة
                    if (request.Currency == Domain.Cash.Enums.Currency.SY)
                        cashRegister.Adjust(-request.Amount, null);
                    else
                        cashRegister.Adjust(0, -request.Amount);
                }
            }

            await unitOfWork.SaveChangesAsync();
            return entity.Id;
        }
    }
}
