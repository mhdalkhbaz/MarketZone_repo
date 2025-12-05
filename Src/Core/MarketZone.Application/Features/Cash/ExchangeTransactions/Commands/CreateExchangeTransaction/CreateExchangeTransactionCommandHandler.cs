using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.DTOs;
using MarketZone.Application.Helpers;
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
        IUnitOfWork unitOfWork,
        ITranslator translator) : IRequestHandler<CreateExchangeTransactionCommand, BaseResult<long>>
    {
        public async Task<BaseResult<long>> Handle(CreateExchangeTransactionCommand request, CancellationToken cancellationToken)
        {
            var cashRegister = await cashRegisterRepository.GetByIdAsync(request.CashRegisterId);
            if (cashRegister == null)
            {
                var message = translator.GetString(TranslatorMessages.CashRegisterMessages.CashRegister_NotFound_with_id(request.CashRegisterId));
                return new Error(ErrorCode.NotFound, message, nameof(request.CashRegisterId));
            }

            // Calculate the converted amount
            decimal toAmount;
            decimal sypDelta;
            decimal usdDelta;
            if (request.Direction == ExchangeDirection.USD_To_SYP)
            {
                toAmount = request.FromAmount * request.ExchangeRate; // USD to SYP
                sypDelta = toAmount;
                usdDelta = -request.FromAmount;

                if (cashRegister.CurrentBalanceDollar + usdDelta < 0)
                {
                    var message = translator.GetString(new TranslatorMessageDto(
                        "Insufficient_Dollar_Balance_In_Cash_Register",
                        [cashRegister.CurrentBalanceDollar.ToString(), request.FromAmount.ToString()]));
                    return new Error(ErrorCode.FieldDataInvalid, message, nameof(request.FromAmount));
                }
            }
            else
            {
                toAmount = request.FromAmount / request.ExchangeRate; // SYP to USD
                sypDelta = -request.FromAmount;
                usdDelta = toAmount;

                if (cashRegister.CurrentBalance + sypDelta < 0)
                {
                    var message = translator.GetString(new TranslatorMessageDto(
                        "Insufficient_Balance_In_Cash_Register",
                        [cashRegister.CurrentBalance.ToString(), request.FromAmount.ToString()]));
                    return new Error(ErrorCode.FieldDataInvalid, message, nameof(request.FromAmount));
                }
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

            // Update cash register balances after passing balance checks
            cashRegister.Adjust(sypDelta, usdDelta);

            await unitOfWork.SaveChangesAsync();
            return entity.Id;
        }
    }
}
