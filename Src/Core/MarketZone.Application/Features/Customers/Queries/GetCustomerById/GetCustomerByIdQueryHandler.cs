using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MarketZone.Application.Helpers;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Customers.DTOs;

namespace MarketZone.Application.Features.Customers.Queries.GetCustomerById
{
	public class GetCustomerByIdQueryHandler(ICustomerRepository customerRepository, ITranslator translator, IMapper mapper) : IRequestHandler<GetCustomerByIdQuery, BaseResult<CustomerDto>>
	{
		public async Task<BaseResult<CustomerDto>> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
		{
			var customer = await customerRepository.GetByIdAsync(request.Id);

			if (customer is null)
			{
				return new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.CustomerMessages.Customer_NotFound_with_id(request.Id)), nameof(request.Id));
			}

			return mapper.Map<CustomerDto>(customer);
		}
	}
}



