using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.Entities;
using MarketZone.Domain.Cash.Enums;
namespace MarketZone.Application.Features.Cash.ExchangeTransactions.Commands.CreateExchangeTransaction
{
    public class CreateExchangeTransactionCommandHandler(
        IExchangeTransactionRepository repository, 
        ICashRegisterRepository cashRegisterRepository,
        IUnitOfWork unitOfWork) : IRequestHandler<CreateExchangeTransactionCommand, BaseResult<long>>
    {
        public async Task<BaseResult<long>> Handle(CreateExchangeTransactionCommand request, CancellationToken cancellationToken)
        {
            // Calculate the converted amount
            decimal toAmount;
            if (request.Direction == ExchangeDirection.USD_To_SYP)
            {
                toAmount = request.FromAmount * request.ExchangeRate; // USD to SYP
            }
            else
            {
                toAmount = request.FromAmount / request.ExchangeRate; // SYP to USD
            }

            // Create exchange transaction
            var entity = new ExchangeTransaction(
                request.CashRegisterId,
                request.Direction,
                request.FromAmount,
                toAmount,
                request.ExchangeRate,
                request.TransactionDate,
                request.Notes
            );

            await repository.AddAsync(entity);

            // Update cash register balances
            var cashRegister = await cashRegisterRepository.GetByIdAsync(request.CashRegisterId);
            if (cashRegister != null)
            {
                if (request.Direction == ExchangeDirection.USD_To_SYP)
                {
                    // Decrease USD, Increase SYP
                    cashRegister.Adjust(toAmount, -request.FromAmount);
                }
                else
                {
                    // Decrease SYP, Increase USD
                    cashRegister.Adjust(-request.FromAmount, toAmount);
                }
            }

            await unitOfWork.SaveChangesAsync();
            return entity.Id;
        }
    }
}
