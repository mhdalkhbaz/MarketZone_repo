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
			// Apply partial updates safely
			entity.UpdateGeneral(
				amount: request.Amount,
				paymentDate: request.PaymentDate,
				description: request.Description,
				notes: request.Notes,
				paidBy: request.PaidBy,
				currency: request.Currency,
				paymentCurrency: request.PaymentCurrency,
				exchangeRate: request.ExchangeRate,
				isConfirmed: request.IsConfirmed);

			if (request.CashRegisterId.HasValue)
				entity.AssignRegister(request.CashRegisterId);

			if (request.Status.HasValue && request.Status.Value == Domain.Cash.Entities.PaymentStatus.Posted)
				entity.Post();

			await unitOfWork.SaveChangesAsync();
			return BaseResult.Ok();
		}
	}
}


