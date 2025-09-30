using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.Entities;
using MarketZone.Domain.Cash.Enums;

namespace MarketZone.Application.Features.Cash.Payments.Commands.CreatePayment
{
	public class CreatePaymentCommandHandler(IPaymentRepository repository, IUnitOfWork unitOfWork) : IRequestHandler<CreatePaymentCommand, BaseResult<long>>
	{
		public async Task<BaseResult<long>> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
		{
			Payment entity;
			
			if (request.PaymentType == PaymentType.InvoicePayment)
			{
				entity = new Payment(
					request.InvoiceId.Value,
					request.Amount,
					request.PaymentDate,
					request.Notes,
					request.ReceivedBy,
					request.IsConfirmed
				);
			}
			else // PaymentType.Expense
			{
				entity = new Payment(
					request.Amount,
					request.PaymentDate ?? System.DateTime.UtcNow,
					request.Description,
					request.PaidBy,
					request.IsConfirmed
				);
			}
			
			entity.AssignRegister(request.CashRegisterId);
			await repository.AddAsync(entity);
			await unitOfWork.SaveChangesAsync();
			return entity.Id;
		}
	}
}


