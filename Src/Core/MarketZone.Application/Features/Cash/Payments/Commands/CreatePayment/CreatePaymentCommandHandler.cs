using System;
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

			// Validate amount
			if (request.Amount <= 0)
			{
				return new Error(ErrorCode.FieldDataInvalid, "Amount must be greater than 0", nameof(request.Amount));
			}
			
			// Check if payment type requires an invoice
			if (RequiresInvoice(request.PaymentType))
			{
				if (!request.InvoiceId.HasValue)
					return new Error(ErrorCode.FieldDataInvalid, "InvoiceId is required for this payment type", nameof(request.InvoiceId));

				entity = new Payment(
					request.PaymentType,
					request.InvoiceId.Value,
                    request.InvoiceType.Value,
                    request.Amount,
					request.PaymentDate ?? DateTime.UtcNow,
					request.Currency,
					request.PaymentCurrency,
					request.ExchangeRate,
					request.Notes,
					request.ReceivedBy,
					request.IsConfirmed
				);
			}
			else // General expenses (not invoice-related)
			{
				entity = new Payment(
					request.PaymentType,
					request.Amount,
					request.PaymentDate ?? DateTime.UtcNow,
					request.Description,
					request.Notes,
					request.PaidBy,
					request.IsConfirmed
				);
			}
			
			entity.AssignRegister(request.CashRegisterId);
			await repository.AddAsync(entity);
			await unitOfWork.SaveChangesAsync();
			return entity.Id;
		}

		private static bool RequiresInvoice(PaymentType type)
		{
			return type == PaymentType.SalesPayment ||
				   type == PaymentType.PurchasePayment ||
				   type == PaymentType.RoastingPayment;
		}
	}
}


