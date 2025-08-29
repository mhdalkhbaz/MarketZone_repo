using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Cash.CashRegisters.Commands.UpdateCashRegister
{
	public class UpdateCashRegisterCommandHandler(ICashRegisterRepository repository, IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<UpdateCashRegisterCommand, BaseResult>
	{
		public async Task<BaseResult> Handle(UpdateCashRegisterCommand request, CancellationToken cancellationToken)
		{
			var entity = await repository.GetByIdAsync(request.Id);
			if (entity is null)
				return BaseResult.Failure();
			mapper.Map(request, entity);
			await unitOfWork.SaveChangesAsync();
			return BaseResult.Ok();
		}
	}
}


