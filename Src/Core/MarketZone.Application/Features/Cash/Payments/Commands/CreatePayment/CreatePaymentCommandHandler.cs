using System;
using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Helpers;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.Entities;
using MarketZone.Domain.Cash.Enums;
using MarketZone.Application.DTOs;

namespace MarketZone.Application.Features.Cash.Payments.Commands.CreatePayment
{
	public class CreatePaymentCommandHandler(
		IPaymentRepository repository, 
		IUnitOfWork unitOfWork,
		IPurchaseInvoiceRepository purchaseInvoiceRepository,
		ISalesInvoiceRepository salesInvoiceRepository,
		IRoastingInvoiceRepository roastingInvoiceRepository,
		IEmployeeRepository employeeRepository,
		ITranslator translator) : IRequestHandler<CreatePaymentCommand, BaseResult<long>>
	{
		public async Task<BaseResult<long>> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
		{
			Payment entity;

		// Validate amount
		if (request.Amount <= 0)
		{
			return new Error(ErrorCode.FieldDataInvalid, translator.GetString("Amount_Must_Be_Greater_Than_0"), nameof(request.Amount));
		}
		
		// Check if payment type requires an invoice
		if (RequiresInvoice(request.PaymentType))
		{
			if (!request.InvoiceId.HasValue)
				return new Error(ErrorCode.FieldDataInvalid, translator.GetString("InvoiceId_Required_For_Payment_Type"), nameof(request.InvoiceId));

				// تحقق من المتبقي قبل إنشاء الدفع
				var currentInvoiceAmount = request.Amount;
				decimal invoiceRemaining = decimal.MaxValue;
				switch (request.PaymentType)
				{
					case PaymentType.PurchasePayment:
					{
						var invoice = await purchaseInvoiceRepository.GetByIdAsync(request.InvoiceId.Value);
						if (invoice == null)
							return new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.PurchaseInvoiceMessages.PurchaseInvoice_NotFound_with_id(request.InvoiceId.Value)), nameof(request.InvoiceId));

						var total = invoice.TotalAmount - invoice.Discount;
						var paid = await repository.GetPostedTotalForInvoiceAsync(invoice.Id, cancellationToken);
						invoiceRemaining = total - paid;
						break;
					}
					case PaymentType.SalesPayment:
					{
						var invoice = await salesInvoiceRepository.GetByIdAsync(request.InvoiceId.Value);
						if (invoice == null)
							return new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.SalesInvoiceMessages.SalesInvoice_NotFound_with_id(request.InvoiceId.Value)), nameof(request.InvoiceId));

						var total = invoice.TotalAmount - invoice.Discount;
						var paid = await repository.GetPostedTotalForInvoiceAsync(invoice.Id, cancellationToken);
						invoiceRemaining = total - paid;
						break;
					}
					case PaymentType.RoastingPayment:
					{
						var invoice = await roastingInvoiceRepository.GetByIdAsync(request.InvoiceId.Value);
						if (invoice == null)
							return new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.SalesInvoiceMessages.SalesInvoice_NotFound_with_id(request.InvoiceId.Value)), nameof(request.InvoiceId));

						var total = invoice.TotalAmount;
						var paid = await repository.GetPostedTotalForInvoiceAsync(invoice.Id, cancellationToken);
						invoiceRemaining = total - paid;
						break;
					}
				}

				if (currentInvoiceAmount > invoiceRemaining)
				{
					var message = translator.GetString("Payment_Amount_Exceeds_Remaining");
					return new Error(ErrorCode.FieldDataInvalid, message, nameof(request.Amount));
				}

				var currency = request.Currency;
				var paymentCurrency = request.PaymentCurrency;

				// For RoastingPayment, automatically set Currency to employee's currency
				if (request.PaymentType == PaymentType.RoastingPayment && request.InvoiceId.HasValue)
				{
					var roastingInvoice = await roastingInvoiceRepository.GetByIdAsync(request.InvoiceId.Value);
					if (roastingInvoice != null && roastingInvoice.EmployeeId.HasValue)
					{
						var employee = await employeeRepository.GetByIdAsync(roastingInvoice.EmployeeId.Value);
						if (employee != null && employee.Currency.HasValue)
						{
							// Set Currency to employee's currency (invoice currency)
							currency = employee.Currency.Value;
							// If PaymentCurrency is not specified or is the same as Currency, keep it as is
							// Otherwise, use the provided PaymentCurrency
							if (paymentCurrency == Currency.SY && currency != Currency.SY)
							{
								// If PaymentCurrency was not explicitly set (default SY), use Currency
								paymentCurrency = currency;
							}
						}
					}
				}

				entity = new Payment(
					request.PaymentType,
					request.InvoiceId.Value,
                    request.InvoiceType.Value,
                    request.Amount,
					request.PaymentDate ?? DateTime.UtcNow,
					currency,
					paymentCurrency,
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
					request.Currency,
					request.PaymentCurrency,
					request.ExchangeRate,
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


