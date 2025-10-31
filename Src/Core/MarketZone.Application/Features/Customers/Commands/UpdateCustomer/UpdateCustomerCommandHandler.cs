using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Helpers;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Customers.Commands.UpdateCustomer
{
	public class UpdateCustomerCommandHandler(ICustomerRepository customerRepository, IUnitOfWork unitOfWork, ITranslator translator) : IRequestHandler<UpdateCustomerCommand, BaseResult>
	{
		public async Task<BaseResult> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
		{
		var customer = await customerRepository.GetByIdAsync(request.Id);

		if (customer is null)
		{
			return new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.CustomerMessages.Customer_NotFound_with_id(request.Id)), nameof(request.Id));
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



