using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.Entities;

namespace MarketZone.Application.Features.Cash.Expenses.Commands.UpdateExpense
{
    public class UpdateExpenseCommandHandler(ICashTransactionRepository repository, ICashRegisterRepository cashRegisterRepository, IUnitOfWork unitOfWork) : IRequestHandler<UpdateExpenseCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(UpdateExpenseCommand request, CancellationToken cancellationToken)
        {
            var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
            if (entity == null)
                return new Error(ErrorCode.NotFound, "Expense not found", nameof(request.Id));

            // Store old values for cash register adjustment
            var oldAmount = entity.Amount;
            var oldTransactionType = entity.TransactionType;
            var oldCashRegisterId = entity.CashRegisterId;

            // Update entity
            entity.Update(
                request.CashRegisterId,
                request.TransactionType,
                request.Amount,
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
                    var oldDelta = oldTransactionType == Domain.Cash.Enums.TransactionType.Income ? -oldAmount : oldAmount;
                    oldCashRegister.Adjust(oldDelta);
                }
            }

            // Adjust new cash register (apply new transaction)
            if (request.CashRegisterId.HasValue)
            {
                var newCashRegister = await cashRegisterRepository.GetByIdAsync(request.CashRegisterId.Value);
                if (newCashRegister != null)
                {
                    var newDelta = request.TransactionType == Domain.Cash.Enums.TransactionType.Income ? request.Amount.Value : -request.Amount.Value;
                    newCashRegister.Adjust(newDelta);
                }
            }

            await unitOfWork.SaveChangesAsync();
            return BaseResult.Ok();
        }
    }
}
