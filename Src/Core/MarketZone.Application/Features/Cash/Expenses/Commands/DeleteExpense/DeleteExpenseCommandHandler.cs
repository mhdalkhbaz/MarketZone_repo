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
                    var delta = entity.TransactionType == Domain.Cash.Enums.TransactionType.Income ? -entity.Amount : entity.Amount;
                    cashRegister.Adjust(delta);
                }
            }

            repository.Delete(entity);
            await unitOfWork.SaveChangesAsync();
            return BaseResult.Ok();
        }
    }
}
