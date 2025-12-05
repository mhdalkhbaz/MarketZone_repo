using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.DTOs;
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

			// Determine new values (without mutating yet)
			var newCashRegisterId = request.CashRegisterId ?? entity.CashRegisterId;
			var newTransactionType = request.TransactionType ?? entity.TransactionType;
			var newAmount = request.Amount ?? entity.Amount;
			var newCurrency = request.Currency ?? entity.Currency;

			// Fetch cash registers
			CashRegister oldCashRegister = null;
			CashRegister newCashRegister = null;
			if (oldCashRegisterId > 0)
			{
				oldCashRegister = await cashRegisterRepository.GetByIdAsync(oldCashRegisterId);
				if (oldCashRegister == null)
					return new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.CashRegisterMessages.CashRegister_NotFound_with_id(oldCashRegisterId)), nameof(entity.CashRegisterId));
			}

			if (newCashRegisterId > 0)
			{
				newCashRegister = oldCashRegisterId == newCashRegisterId
					? oldCashRegister
					: await cashRegisterRepository.GetByIdAsync(newCashRegisterId);

				if (newCashRegister == null)
					return new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.CashRegisterMessages.CashRegister_NotFound_with_id(newCashRegisterId)), nameof(request.CashRegisterId));
			}

			// Helpers to compute effect (+ add, - withdraw)
			static decimal Effect(Domain.Cash.Enums.TransactionType type, decimal amount)
				=> type == Domain.Cash.Enums.TransactionType.Income ? amount : -amount;

			var oldEffectSy = oldCurrency == Domain.Cash.Enums.Currency.SY ? Effect(oldTransactionType, oldAmount) : 0;
			var oldEffectDollar = oldCurrency == Domain.Cash.Enums.Currency.Dollar ? Effect(oldTransactionType, oldAmount) : 0;

			var newEffectSy = newCurrency == Domain.Cash.Enums.Currency.SY ? Effect(newTransactionType, newAmount) : 0;
			var newEffectDollar = newCurrency == Domain.Cash.Enums.Currency.Dollar ? Effect(newTransactionType, newAmount) : 0;

			if (oldCashRegisterId == newCashRegisterId && newCashRegister != null)
			{
				var deltaSy = -oldEffectSy + newEffectSy;
				var deltaDollar = -oldEffectDollar + newEffectDollar;

				if (deltaSy < 0 && newCashRegister.CurrentBalance + deltaSy < 0)
				{
					var message = translator.GetString(new TranslatorMessageDto(
						"Insufficient_Balance_In_Cash_Register",
						[newCashRegister.CurrentBalance.ToString(), newAmount.ToString()]));
					return new Error(ErrorCode.FieldDataInvalid, message, nameof(request.Amount));
				}

				if (deltaDollar < 0 && newCashRegister.CurrentBalanceDollar + deltaDollar < 0)
				{
					var message = translator.GetString(new TranslatorMessageDto(
						"Insufficient_Dollar_Balance_In_Cash_Register",
						[newCashRegister.CurrentBalanceDollar.ToString(), newAmount.ToString()]));
					return new Error(ErrorCode.FieldDataInvalid, message, nameof(request.Amount));
				}
			}
			else
			{
				// Check old register reversal (can withdraw if reversing income)
				if (oldCashRegister != null)
				{
					var deltaOldSy = -oldEffectSy;
					var deltaOldDollar = -oldEffectDollar;

					if (deltaOldSy < 0 && oldCashRegister.CurrentBalance + deltaOldSy < 0)
					{
						var message = translator.GetString(new TranslatorMessageDto(
							"Insufficient_Balance_In_Cash_Register",
							[oldCashRegister.CurrentBalance.ToString(), oldAmount.ToString()]));
						return new Error(ErrorCode.FieldDataInvalid, message, nameof(request.Amount));
					}

					if (deltaOldDollar < 0 && oldCashRegister.CurrentBalanceDollar + deltaOldDollar < 0)
					{
						var message = translator.GetString(new TranslatorMessageDto(
							"Insufficient_Dollar_Balance_In_Cash_Register",
							[oldCashRegister.CurrentBalanceDollar.ToString(), oldAmount.ToString()]));
						return new Error(ErrorCode.FieldDataInvalid, message, nameof(request.Amount));
					}
				}

				// Check new register application
				if (newCashRegister != null)
				{
					if (newEffectSy < 0 && newCashRegister.CurrentBalance + newEffectSy < 0)
					{
						var message = translator.GetString(new TranslatorMessageDto(
							"Insufficient_Balance_In_Cash_Register",
							[newCashRegister.CurrentBalance.ToString(), newAmount.ToString()]));
						return new Error(ErrorCode.FieldDataInvalid, message, nameof(request.Amount));
					}

					if (newEffectDollar < 0 && newCashRegister.CurrentBalanceDollar + newEffectDollar < 0)
					{
						var message = translator.GetString(new TranslatorMessageDto(
							"Insufficient_Dollar_Balance_In_Cash_Register",
							[newCashRegister.CurrentBalanceDollar.ToString(), newAmount.ToString()]));
						return new Error(ErrorCode.FieldDataInvalid, message, nameof(request.Amount));
					}
				}
			}

			// Update entity after validations
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

			// Apply adjustments
			if (oldCashRegisterId == newCashRegisterId && newCashRegister != null)
			{
				var deltaSy = -oldEffectSy + newEffectSy;
				var deltaDollar = -oldEffectDollar + newEffectDollar;
				newCashRegister.Adjust(deltaSy, deltaDollar);
			}
			else
			{
				if (oldCashRegister != null)
				{
					oldCashRegister.Adjust(-oldEffectSy, -oldEffectDollar);
				}

				if (newCashRegister != null)
				{
					newCashRegister.Adjust(newEffectSy, newEffectDollar);
				}
			}

			await unitOfWork.SaveChangesAsync();
			return BaseResult.Ok();
        }
    }
}
