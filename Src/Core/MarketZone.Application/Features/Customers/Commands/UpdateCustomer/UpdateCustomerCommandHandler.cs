using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MarketZone.Application.Helpers;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Customers.Commands.UpdateCustomer
{
	public class UpdateCustomerCommandHandler(ICustomerRepository customerRepository, IUnitOfWork unitOfWork, ITranslator translator, IMapper mapper) : IRequestHandler<UpdateCustomerCommand, BaseResult>
	{
		public async Task<BaseResult> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
		{
			var customer = await customerRepository.GetByIdAsync(request.Id);

			if (customer is null)
			{
				return new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.CustomerMessages.Customer_NotFound_with_id(request.Id)), nameof(request.Id));
			}

			mapper.Map(request, customer);
			await unitOfWork.SaveChangesAsync();

			return BaseResult.Ok();
		}
	}
}



