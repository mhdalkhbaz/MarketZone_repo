using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Helpers;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Customers.Commands.DeleteCustomer
{
	public class DeleteCustomerCommandHandler(ICustomerRepository customerRepository, IUnitOfWork unitOfWork, ITranslator translator) : IRequestHandler<DeleteCustomerCommand, BaseResult>
	{
		public async Task<BaseResult> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
		{
			var customer = await customerRepository.GetByIdAsync(request.Id);

			if (customer is null)
			{
				return new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.CustomerMessages.Customer_NotFound_with_id(request.Id)), nameof(request.Id));
			}

			customerRepository.Delete(customer);
			await unitOfWork.SaveChangesAsync();

			return BaseResult.Ok();
		}
	}
}



