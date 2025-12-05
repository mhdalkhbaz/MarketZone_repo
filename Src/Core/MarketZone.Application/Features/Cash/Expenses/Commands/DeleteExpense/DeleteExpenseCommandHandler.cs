using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.DTOs;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Application.Helpers;

namespace MarketZone.Application.Features.Cash.Expenses.Commands.DeleteExpense
{
	public class DeleteExpenseCommandHandler(ICashTransactionRepository repository, ICashRegisterRepository cashRegisterRepository, IUnitOfWork unitOfWork, ITranslator translator) : IRequestHandler<DeleteExpenseCommand, BaseResult>
	{
		public async Task<BaseResult> Handle(DeleteExpenseCommand request, CancellationToken cancellationToken)
		{
			var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
			if (entity == null)
				return new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.ExpenseMessages.Expense_NotFound_with_id(request.Id)), nameof(request.Id));

            // Adjust cash register (reverse transaction)
			if (entity.CashRegisterId > 0)
			{
				var cashRegister = await cashRegisterRepository.GetByIdAsync(entity.CashRegisterId);
				if (cashRegister == null)
				{
					var message = translator.GetString(TranslatorMessages.CashRegisterMessages.CashRegister_NotFound_with_id(entity.CashRegisterId));
					return new Error(ErrorCode.NotFound, message, nameof(entity.CashRegisterId));
				}

				// عكس العملية حسب نوعها وعملتها
				if (entity.TransactionType == Domain.Cash.Enums.TransactionType.Income)
				{
					// كان دخل، ننقصه (تحقق من الرصيد قبل السحب)
					if (entity.Currency == Domain.Cash.Enums.Currency.SY)
					{
						if (cashRegister.CurrentBalance < entity.Amount)
						{
							var message = translator.GetString(new TranslatorMessageDto(
								"Insufficient_Balance_In_Cash_Register",
								[cashRegister.CurrentBalance.ToString(), entity.Amount.ToString()]));
							return new Error(ErrorCode.FieldDataInvalid, message, nameof(entity.Amount));
						}
						cashRegister.Adjust(-entity.Amount, null);
					}
					else
					{
						if (cashRegister.CurrentBalanceDollar < entity.Amount)
						{
							var message = translator.GetString(new TranslatorMessageDto(
								"Insufficient_Dollar_Balance_In_Cash_Register",
								[cashRegister.CurrentBalanceDollar.ToString(), entity.Amount.ToString()]));
							return new Error(ErrorCode.FieldDataInvalid, message, nameof(entity.Amount));
						}
						cashRegister.Adjust(0, -entity.Amount);
					}
				}
				else // Expense
				{
					// كان مصروف، نرجعه
					if (entity.Currency == Domain.Cash.Enums.Currency.SY)
						cashRegister.Adjust(entity.Amount, null);
					else
						cashRegister.Adjust(0, entity.Amount);
				}
			}

            repository.Delete(entity);
            await unitOfWork.SaveChangesAsync();
            return BaseResult.Ok();
        }
    }
}
