using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.Entities;

namespace MarketZone.Application.Features.Cash.ExchangeRates.Commands.CreateExchangeRate
{
	public class CreateExchangeRateCommandHandler : IRequestHandler<CreateExchangeRateCommand, BaseResult<long>>
	{
		private readonly IExchangeRateRepository _repository;
		private readonly IUnitOfWork _unitOfWork;

		public CreateExchangeRateCommandHandler(IExchangeRateRepository repository, IUnitOfWork unitOfWork)
		{
			_repository = repository;
			_unitOfWork = unitOfWork;
		}

		public async Task<BaseResult<long>> Handle(CreateExchangeRateCommand request, CancellationToken cancellationToken)
		{
			var entity = new ExchangeRate(request.CurrencyCode, request.RateToUSD, request.EffectiveAtUtc, request.Source, request.Notes);
			await _repository.AddAsync(entity);
			await _unitOfWork.SaveChangesAsync();
			return entity.Id;
		}
	}
}


