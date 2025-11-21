using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Application.DTOs;

namespace MarketZone.Application.Features.Cash.Payments.Commands.UpdatePayment
{
	public class UpdatePaymentCommandHandler(IPaymentRepository repository, IUnitOfWork unitOfWork, IMapper mapper, ITranslator translator) : IRequestHandler<UpdatePaymentCommand, BaseResult>
	{
		public async Task<BaseResult> Handle(UpdatePaymentCommand request, CancellationToken cancellationToken)
		{
			var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
			if (entity is null)
				return BaseResult.Failure();
			if (entity.Status == Domain.Cash.Entities.PaymentStatus.Posted)
				return new Error(ErrorCode.AccessDenied, translator.GetString("Cannot_Update_Posted_Payment"), nameof(request.Id));
			mapper.Map(request, entity);
			await unitOfWork.SaveChangesAsync();
			return BaseResult.Ok();
		}
	}
}


