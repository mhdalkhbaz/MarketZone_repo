using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Helpers;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.Entities;
using MarketZone.Domain.Cash.Enums;
using MarketZone.Domain.Purchases.Enums;
using MarketZone.Application.DTOs;

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
            return new Error(ErrorCode.FieldDataInvalid, translator.GetString("PostedInvoice"), nameof(payment.CashRegisterId));


        if (!payment.CashRegisterId.HasValue)
			return new Error(ErrorCode.FieldDataInvalid, translator.GetString("CashRegisterId_Required_To_Post_Payment"), nameof(payment.CashRegisterId));

		// التحقق من رصيد الصندوق قبل الدفع
		var cashRegister = await cashRegisterRepository.GetByIdAsync(payment.CashRegisterId.Value);
		if (cashRegister == null)
			return new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.CashRegisterMessages.CashRegister_NotFound_with_id(payment.CashRegisterId.Value)), nameof(payment.CashRegisterId));

			// استخدام PaymentCurrency (العملة التي تم الدفع بها فعلياً)
			var transactionCurrency = payment.PaymentCurrency;
			
			// SalesPayment = Income (إيرادات) - يضيف على الصندوق
			// باقي الأنواع = Expense (مصروفات) - يخرج من الصندوق
			var isIncome = payment.PaymentType == PaymentType.SalesPayment;
			
			// التحقق من أن الرصيد كافي فقط للمصروفات (Expense)
			if (!isIncome)
			{
				if (transactionCurrency == Currency.SY)
				{
			if (cashRegister.CurrentBalance < payment.Amount)
			{
				var message = translator.GetString(new TranslatorMessageDto("Insufficient_Balance_In_Cash_Register", 
					new[] { cashRegister.CurrentBalance.ToString(), payment.Amount.ToString() }));
				return new Error(ErrorCode.FieldDataInvalid, message, nameof(payment.Amount));
			}
			}
			else if (transactionCurrency == Currency.Dollar)
			{
				if (cashRegister.CurrentBalanceDollar < payment.Amount)
				{
					var message = translator.GetString(new TranslatorMessageDto("Insufficient_Dollar_Balance_In_Cash_Register", 
						new[] { cashRegister.CurrentBalanceDollar.ToString(), payment.Amount.ToString() }));
					return new Error(ErrorCode.FieldDataInvalid, message, nameof(payment.Amount));
				}
			}
			}

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

			// Create cash transaction
			// SalesPayment = Income (إيرادات) - يضيف على الصندوق
			// باقي الأنواع = Expense (مصروفات) - يخرج من الصندوق
			var transactionType = isIncome ? Domain.Cash.Enums.TransactionType.Income : Domain.Cash.Enums.TransactionType.Expense;
			var cashTransaction = new CashTransaction(
				payment.CashRegisterId.Value,
				transactionType,
				payment.Amount,
				transactionCurrency,
				payment.PaymentDate,
				referenceType,
				payment.Id,
				transactionDescription);

			await cashTransactionRepository.AddAsync(cashTransaction);

			// Adjust cash register حسب نوع الدفعة والعملة
			if (isIncome)
			{
				// إيرادات: إضافة على الصندوق
				if (transactionCurrency == Currency.SY)
					cashRegister.Adjust(payment.Amount, null);
				else
					cashRegister.Adjust(0, payment.Amount);
			}
			else
			{
				// مصروفات: إنقاص من الصندوق
				if (transactionCurrency == Currency.SY)
					cashRegister.Adjust(-payment.Amount, null);
				else
					cashRegister.Adjust(0, -payment.Amount);
			}

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
				await UpdateEmployeeBalanceForRoastingPayment(payment, cancellationToken);
			}

			payment.Post();
			await unitOfWork.SaveChangesAsync();
			return BaseResult.Ok();
		}

		private async Task UpdateEmployeeBalanceForRoastingPayment(Payment payment, CancellationToken cancellationToken)
		{
			if (!payment.InvoiceId.HasValue)
				return;

			var roastingInvoice = await roastingInvoiceRepository.GetByIdAsync(payment.InvoiceId.Value);
			if (roastingInvoice == null || !roastingInvoice.EmployeeId.HasValue)
				return;

			var employee = await employeeRepository.GetByIdAsync(roastingInvoice.EmployeeId.Value);
			if (employee == null)
				return;

			// تحديد المبلغ والعملة للدفع
			// PaymentCurrency هي العملة التي تم الدفع بها فعلياً
			// Amount هو المبلغ بالعملة المدفوعة (PaymentCurrency)
			var paymentAmount = payment.Amount;
			var paymentCurrency = payment.PaymentCurrency;
			
			var currentSyrianMoney = employee.SyrianMoney ?? 0;
			var currentDollarMoney = employee.DollarMoney ?? 0;

			// تحديث الرصيد حسب العملة التي تم الدفع بها
			if (paymentCurrency == Currency.SY)
			{
				// الدفع بالليرة السورية
				currentSyrianMoney += paymentAmount;
			}
			else if (paymentCurrency == Currency.Dollar)
			{
				// الدفع بالدولار
				currentDollarMoney += paymentAmount;
			}
			else
			{
				// افتراضياً: إذا لم تحدد PaymentCurrency، نستخدم Currency (عملة الفاتورة)
				if (payment.Currency == Currency.SY)
				{
					currentSyrianMoney += paymentAmount;
				}
				else if (payment.Currency == Currency.Dollar)
				{
					currentDollarMoney += paymentAmount;
				}
			}

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
				currentSyrianMoney,
				currentDollarMoney
			);

			employeeRepository.Update(employee);
		}
	}
}


