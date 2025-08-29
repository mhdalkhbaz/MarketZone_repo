using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MarketZone.Application.Helpers;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Employees.DTOs;

namespace MarketZone.Application.Features.Employees.Queries.GetEmployeeById
{
	public class GetEmployeeByIdQueryHandler(IEmployeeRepository employeeRepository, ITranslator translator, IMapper mapper) : IRequestHandler<GetEmployeeByIdQuery, BaseResult<EmployeeDto>>
	{
		public async Task<BaseResult<EmployeeDto>> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
		{
			var employee = await employeeRepository.GetByIdAsync(request.Id);

			if (employee is null)
			{
				return new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.EmployeeMessages.Employee_NotFound_with_id(request.Id)), nameof(request.Id));
			}

			return mapper.Map<EmployeeDto>(employee);
		}
	}
}



