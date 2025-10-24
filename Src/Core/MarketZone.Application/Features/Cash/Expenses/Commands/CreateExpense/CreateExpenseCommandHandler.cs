using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.Entities;
using MarketZone.Domain.Cash.Enums;

namespace MarketZone.Application.Features.Cash.Expenses.Commands.CreateExpense
{
    public class CreateExpenseCommandHandler(ICashTransactionRepository repository, ICashRegisterRepository cashRegisterRepository, IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<CreateExpenseCommand, BaseResult<long>>
    {
        public async Task<BaseResult<long>> Handle(CreateExpenseCommand request, CancellationToken cancellationToken)
        {
            var entity = mapper.Map<CashTransaction>(request);
            await repository.AddAsync(entity);

            var cashRegister = await cashRegisterRepository.GetByIdAsync(request.CashRegisterId);
            if (cashRegister != null)
            {
                var delta = request.TransactionType == TransactionType.Income ? request.Amount : -request.Amount;
                cashRegister.Adjust(delta);
            }

            await unitOfWork.SaveChangesAsync();
            return entity.Id;
        }
    }
}
