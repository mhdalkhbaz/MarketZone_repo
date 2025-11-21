using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.Entities;
using MarketZone.Application.Helpers;

namespace MarketZone.Application.Features.Cash.Expenses.Commands.UpdateExpense
{
	public class UpdateExpenseCommandHandler(ICashTransactionRepository repository, ICashRegisterRepository cashRegisterRepository, IUnitOfWork unitOfWork, ITranslator translator) : IRequestHandler<UpdateExpenseCommand, BaseResult>
	{
		public async Task<BaseResult> Handle(UpdateExpenseCommand request, CancellationToken cancellationToken)
		{
			var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
			if (entity == null)
				return new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.ExpenseMessages.Expense_NotFound_with_id(request.Id)), nameof(request.Id));

            // Store old values for cash register adjustment
            var oldAmount = entity.Amount;
            var oldTransactionType = entity.TransactionType;
            var oldCurrency = entity.Currency;
            var oldCashRegisterId = entity.CashRegisterId;

            // Update entity
            entity.Update(
                request.CashRegisterId,
                request.TransactionType,
                request.Amount,
                request.Currency,
                request.TransactionDate,
                request.ReferenceType,
                request.ReferenceId,
                request.Description
            );

            // Adjust old cash register (reverse old transaction)
            if (oldCashRegisterId > 0)
            {
                var oldCashRegister = await cashRegisterRepository.GetByIdAsync(oldCashRegisterId);
                if (oldCashRegister != null)
                {
                    // عكس العملية القديمة
                    if (oldTransactionType == Domain.Cash.Enums.TransactionType.Income)
                    {
                        // كان دخل، ننقصه
                        if (oldCurrency == Domain.Cash.Enums.Currency.SY)
                            oldCashRegister.Adjust(-oldAmount, null);
                        else
                            oldCashRegister.Adjust(0, -oldAmount);
                    }
                    else // Expense
                    {
                        // كان مصروف، نرجعه
                        if (oldCurrency == Domain.Cash.Enums.Currency.SY)
                            oldCashRegister.Adjust(oldAmount, null);
                        else
                            oldCashRegister.Adjust(0, oldAmount);
                    }
                }
            }

            // Adjust new cash register (apply new transaction)
            var newCashRegisterId = request.CashRegisterId ?? entity.CashRegisterId;
            var newTransactionType = request.TransactionType ?? entity.TransactionType;
            var newAmount = request.Amount ?? entity.Amount;
            var newCurrency = request.Currency ?? entity.Currency;

            if (newCashRegisterId > 0)
            {
                var newCashRegister = await cashRegisterRepository.GetByIdAsync(newCashRegisterId);
                if (newCashRegister != null)
                {
                    // تطبيق العملية الجديدة
                    if (newTransactionType == Domain.Cash.Enums.TransactionType.Income)
                    {
                        // دخل، نضيفه
                        if (newCurrency == Domain.Cash.Enums.Currency.SY)
                            newCashRegister.Adjust(newAmount, null);
                        else
                            newCashRegister.Adjust(0, newAmount);
                    }
                    else // Expense
                    {
                        // مصروف، ننقصه
                        if (newCurrency == Domain.Cash.Enums.Currency.SY)
                            newCashRegister.Adjust(-newAmount, null);
                        else
                            newCashRegister.Adjust(0, -newAmount);
                    }
                }
            }

            await unitOfWork.SaveChangesAsync();
            return BaseResult.Ok();
        }
    }
}
