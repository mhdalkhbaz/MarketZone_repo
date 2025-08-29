using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Customers.DTOs;

namespace MarketZone.Application.Features.Customers.Queries.GetCustomerById
{
	public class GetCustomerByIdQuery : IRequest<BaseResult<CustomerDto>>
	{
		public long Id { get; set; }
	}
}



