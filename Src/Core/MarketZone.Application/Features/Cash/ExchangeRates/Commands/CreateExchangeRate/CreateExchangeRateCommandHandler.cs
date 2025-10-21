using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.Entities;

namespace MarketZone.Application.Features.Cash.ExchangeRates.Commands.CreateExchangeRate
{
    public class CreateExchangeRateCommandHandler(IExchangeRateRepository repository, IUnitOfWork unitOfWork) : IRequestHandler<CreateExchangeRateCommand, BaseResult<long>>
    {
        public async Task<BaseResult<long>> Handle(CreateExchangeRateCommand request, CancellationToken cancellationToken)
        {
            // Deactivate all existing rates
            var existingRates = await repository.GetActiveRatesAsync(cancellationToken);
            foreach (var rate in existingRates)
            {
                rate.Deactivate();
            }

            // Create new exchange rate
            var entity = new ExchangeRate(request.Rate, request.EffectiveDate);
            await repository.AddAsync(entity);
            await unitOfWork.SaveChangesAsync();
            
            return entity.Id;
        }
    }
}