using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MarketZone.Application.Helpers;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.Entities;
using MarketZone.Domain.Cash.Enums;
using MarketZone.Domain.Purchases.Enums;

namespace MarketZone.Application.Features.Cash.Payments.Commands.PostPayment
{
	public class PostPaymentCommandHandler(
		IPaymentRepository paymentRepository,
		ICashTransactionRepository cashTransactionRepository,
		ICashRegisterRepository cashRegisterRepository,
		IPurchaseInvoiceRepository purchaseInvoiceRepository,
		IUnitOfWork unitOfWork,
		ITranslator translator,
		IMapper mapper
	) : IRequestHandler<PostPaymentCommand, BaseResult>
	{
		public async Task<BaseResult> Handle(PostPaymentCommand request, CancellationToken cancellationToken)
		{
			var payment = await paymentRepository.GetByIdAsync(request.Id, cancellationToken);
			if (payment is null)
				return new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.PurchaseInvoiceMessages.PurchaseInvoice_NotFound_with_id(request.Id)), nameof(request.Id));

			if (payment.Status == PaymentStatus.Posted)
				return BaseResult.Ok();

			if (!payment.CashRegisterId.HasValue)
				return new Error(ErrorCode.FieldDataInvalid, "CashRegisterId is required to post a payment", nameof(payment.CashRegisterId));

			// For purchase invoice payments, cash goes OUT → Expense transaction
			var cashTransaction = new CashTransaction(
				payment.CashRegisterId.Value,
				TransactionType.Expense,
				payment.Amount,
				payment.PaymentDate,
				ReferenceType.Payment,
				payment.Id,
				$"Payment for purchase invoice {payment.InvoiceId}");

			await cashTransactionRepository.AddAsync(cashTransaction);

			// Adjust cash register (decrease)
			var cashRegister = await cashRegisterRepository.GetByIdAsync(payment.CashRegisterId.Value);
			cashRegister?.Adjust(-payment.Amount);

			// Update invoice payment status (Purchase invoice assumed)
			var invoice = await purchaseInvoiceRepository.GetByIdAsync(payment.InvoiceId);
			if (invoice != null)
			{
				var totalPosted = await paymentRepository.GetPostedTotalForInvoiceAsync(payment.InvoiceId, cancellationToken);
				var totalAfter = totalPosted + payment.Amount;
				var due = invoice.TotalAmount - invoice.Discount;
				invoice.SetPaymentStatus(totalAfter >= due ? PurchasePaymentStatus.CompletePayment : PurchasePaymentStatus.PartialPayment);
			}

			payment.Post();
			await unitOfWork.SaveChangesAsync();
			return BaseResult.Ok();
		}
	}
}


