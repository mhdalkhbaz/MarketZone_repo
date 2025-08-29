using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Helpers;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Employees.Commands.DeleteEmployee
{
	public class DeleteEmployeeCommandHandler(IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork, ITranslator translator) : IRequestHandler<DeleteEmployeeCommand, BaseResult>
	{
		public async Task<BaseResult> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
		{
			var employee = await employeeRepository.GetByIdAsync(request.Id);

			if (employee is null)
			{
				return new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.EmployeeMessages.Employee_NotFound_with_id(request.Id)), nameof(request.Id));
			}

			employeeRepository.Delete(employee);
			await unitOfWork.SaveChangesAsync();

			return BaseResult.Ok();
		}
	}
}



