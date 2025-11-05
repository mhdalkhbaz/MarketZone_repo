using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Helpers;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Application.Interfaces.Repositories;

namespace MarketZone.Application.Features.Customers.Commands.UpdateCustomer
{
    public class UpdateCustomerCommandHandler(ICustomerRepository customerRepository, ISalesInvoiceRepository salesInvoiceRepository, IUnitOfWork unitOfWork, ITranslator translator) : IRequestHandler<UpdateCustomerCommand, BaseResult>
	{
		public async Task<BaseResult> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
		{
		var customer = await customerRepository.GetByIdAsync(request.Id);

		if (customer is null)
		{
			return new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.CustomerMessages.Customer_NotFound_with_id(request.Id)), nameof(request.Id));
		}

		// Prevent changing currency if customer is linked to any other operation (e.g., sales invoices)
		if (request.Currency.HasValue && request.Currency != customer.Currency)
		{
			var hasLinkedInvoices = await salesInvoiceRepository.HasInvoicesForCustomerAsync(customer.Id, cancellationToken);
			if (hasLinkedInvoices)
			{
				return new Error(ErrorCode.FieldDataInvalid, translator.GetString(TranslatorMessages.CustomerMessages.Cannot_change_currency_when_customer_has_operations()), nameof(request.Currency));
			}
		}

		customer.Update(
			request.Name ?? customer.Name,
			request.Phone ?? customer.Phone,
			request.WhatsAppPhone ?? customer.WhatsAppPhone,
			request.Email ?? customer.Email,
			request.Address ?? customer.Address,
			request.Currency ?? customer.Currency,
			request.IsActive ?? customer.IsActive
		);
		await unitOfWork.SaveChangesAsync();

			return BaseResult.Ok();
		}
	}
}



