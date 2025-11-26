using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Customers.DTOs;

namespace MarketZone.Application.Features.Customers.Queries.GetPagedListCustomer
{
	public class GetPagedListCustomerQueryHandler(ICustomerRepository customerRepository) : IRequestHandler<GetPagedListCustomerQuery, PagedResponse<CustomerDto>>
	{
		public async Task<PagedResponse<CustomerDto>> Handle(GetPagedListCustomerQuery request, CancellationToken cancellationToken)
		{
			return await customerRepository.GetPagedListAsync(request);
		}
	}
}



