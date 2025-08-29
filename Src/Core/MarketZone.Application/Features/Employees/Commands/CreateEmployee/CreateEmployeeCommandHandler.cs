using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Employees.Entities;

namespace MarketZone.Application.Features.Employees.Commands.CreateEmployee
{
	public class CreateEmployeeCommandHandler(IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<CreateEmployeeCommand, BaseResult<long>>
	{
		public async Task<BaseResult<long>> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
		{
			var employee = mapper.Map<Employee>(request);

			await employeeRepository.AddAsync(employee);
			await unitOfWork.SaveChangesAsync();

			return employee.Id;
		}
	}
}



