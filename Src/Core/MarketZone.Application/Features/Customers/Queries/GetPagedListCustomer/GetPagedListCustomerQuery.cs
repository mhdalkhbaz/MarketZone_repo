using MarketZone.Application.Interfaces;
using MarketZone.Application.Parameters;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Customers.DTOs;

namespace MarketZone.Application.Features.Customers.Queries.GetPagedListCustomer
{
	public class GetPagedListCustomerQuery : CustomerFilter, IRequest<PagedResponse<CustomerDto>>
	{
	}
}



