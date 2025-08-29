using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.Entities;

namespace MarketZone.Application.Features.Cash.CashRegisters.Commands.CreateCashRegister
{
	public class CreateCashRegisterCommandHandler(ICashRegisterRepository repository, IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<CreateCashRegisterCommand, BaseResult<long>>
	{
		public async Task<BaseResult<long>> Handle(CreateCashRegisterCommand request, CancellationToken cancellationToken)
		{
			var entity = mapper.Map<CashRegister>(request);
			await repository.AddAsync(entity);
			await unitOfWork.SaveChangesAsync();
			return entity.Id;
		}
	}
}


