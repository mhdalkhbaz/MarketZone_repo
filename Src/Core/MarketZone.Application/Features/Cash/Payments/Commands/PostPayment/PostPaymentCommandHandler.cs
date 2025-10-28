using System.Threading;
using System.Threading.Tasks;
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
		IEmployeeRepository employeeRepository,
		IRoastingInvoiceRepository roastingInvoiceRepository,
		IUnitOfWork unitOfWork,
		ITranslator translator
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

			string transactionDescription;
			Domain.Cash.Enums.ReferenceType referenceType;

			if (payment.PaymentType == Domain.Cash.Enums.PaymentType.SalesPayment || 
			    payment.PaymentType == Domain.Cash.Enums.PaymentType.PurchasePayment || 
			    payment.PaymentType == Domain.Cash.Enums.PaymentType.RoastingPayment)
			{
				transactionDescription = $"Payment for invoice {payment.InvoiceId}";
				referenceType = Domain.Cash.Enums.ReferenceType.Payment;
			}
			else // General expenses
			{
				transactionDescription = payment.Description ?? "Expense payment";
				referenceType = Domain.Cash.Enums.ReferenceType.Expense;
			}

			// Create cash transaction (cash goes OUT → Expense transaction)
			var cashTransaction = new CashTransaction(
				payment.CashRegisterId.Value,
				Domain.Cash.Enums.TransactionType.Expense,
				payment.Amount,
				payment.PaymentDate,
				referenceType,
				payment.Id,
				transactionDescription);

			await cashTransactionRepository.AddAsync(cashTransaction);

			// Adjust cash register (decrease)
			var cashRegister = await cashRegisterRepository.GetByIdAsync(payment.CashRegisterId.Value);
			cashRegister?.Adjust(-payment.Amount);

			// Update invoice payment status (only for invoice payments)
			if ((payment.PaymentType == Domain.Cash.Enums.PaymentType.SalesPayment || 
			     payment.PaymentType == Domain.Cash.Enums.PaymentType.PurchasePayment || 
			     payment.PaymentType == Domain.Cash.Enums.PaymentType.RoastingPayment) && payment.InvoiceId.HasValue)
			{
				var invoice = await purchaseInvoiceRepository.GetByIdAsync(payment.InvoiceId.Value);
				if (invoice != null)
				{
					var totalPosted = await paymentRepository.GetPostedTotalForInvoiceAsync(payment.InvoiceId.Value, cancellationToken);
					var totalAfter = totalPosted + payment.Amount;
					var due = invoice.TotalAmount - invoice.Discount;
					invoice.SetPaymentStatus(totalAfter >= due ? PurchasePaymentStatus.CompletePayment : PurchasePaymentStatus.PartialPayment);
				}
			}

			// Update employee balance for roasting payment
			if (payment.PaymentType == Domain.Cash.Enums.PaymentType.RoastingPayment && payment.InvoiceId.HasValue)
			{
				await UpdateEmployeeBalanceForRoastingPayment(payment.InvoiceId.Value, payment.Amount, cancellationToken);
			}

			payment.Post();
			await unitOfWork.SaveChangesAsync();
			return BaseResult.Ok();
		}

		private async Task UpdateEmployeeBalanceForRoastingPayment(long roastingInvoiceId, decimal paymentAmount, CancellationToken cancellationToken)
		{
			var roastingInvoice = await roastingInvoiceRepository.GetByIdAsync(roastingInvoiceId);
			if (roastingInvoice == null || !roastingInvoice.EmployeeId.HasValue)
				return;

			var employee = await employeeRepository.GetByIdAsync(roastingInvoice.EmployeeId.Value);
			if (employee == null)
				return;

			// Add payment amount to employee's Syrian money balance
			var currentSyrianMoney = employee.SyrianMoney ?? 0;
			employee.Update(
				employee.FirstName,
				employee.LastName,
				employee.Phone,
				employee.WhatsAppPhone,
				employee.Email,
				employee.Address,
				employee.JobTitle,
				employee.Salary,
				employee.HireDate,
				employee.IsActive,
				currentSyrianMoney + paymentAmount,
				employee.DollarMoney
			);

			employeeRepository.Update(employee);
		}
	}
}


