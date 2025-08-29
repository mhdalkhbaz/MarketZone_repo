using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Customers.Entities;

namespace MarketZone.Application.Features.Customers.Commands.CreateCustomer
{
	public class CreateCustomerCommandHandler(ICustomerRepository customerRepository, IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<CreateCustomerCommand, BaseResult<long>>
	{
		public async Task<BaseResult<long>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
		{
			var customer = mapper.Map<Customer>(request);

			await customerRepository.AddAsync(customer);
			await unitOfWork.SaveChangesAsync();

			return customer.Id;
		}
	}
}



