using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Cash.Expenses.Commands.DeleteExpense
{
    public class DeleteExpenseCommandHandler(ICashTransactionRepository repository, ICashRegisterRepository cashRegisterRepository, IUnitOfWork unitOfWork) : IRequestHandler<DeleteExpenseCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(DeleteExpenseCommand request, CancellationToken cancellationToken)
        {
            var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
            if (entity == null)
                return new Error(ErrorCode.NotFound, "Expense not found", nameof(request.Id));

            // Adjust cash register (reverse transaction)
            if (entity.CashRegisterId > 0)
            {
                var cashRegister = await cashRegisterRepository.GetByIdAsync(entity.CashRegisterId);
                if (cashRegister != null)
                {
                    // عكس العملية حسب نوعها وعملتها
                    if (entity.TransactionType == Domain.Cash.Enums.TransactionType.Income)
                    {
                        // كان دخل، ننقصه
                        if (entity.Currency == Domain.Cash.Enums.Currency.SY)
                            cashRegister.Adjust(-entity.Amount, null);
                        else
                            cashRegister.Adjust(0, -entity.Amount);
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
            }

            repository.Delete(entity);
            await unitOfWork.SaveChangesAsync();
            return BaseResult.Ok();
        }
    }
}
