using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Helpers;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Employees.Commands.DeleteEmployee
{
	public class DeleteEmployeeCommandHandler(
		IEmployeeRepository employeeRepository, 
		IUnitOfWork unitOfWork, 
		ITranslator translator,
		IRoastingInvoiceRepository roastingInvoiceRepository) : IRequestHandler<DeleteEmployeeCommand, BaseResult>
	{
		public async Task<BaseResult> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
		{
			var employee = await employeeRepository.GetByIdAsync(request.Id);

			if (employee is null)
			{
				return new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.EmployeeMessages.Employee_NotFound_with_id(request.Id)), nameof(request.Id));
			}

			// التحقق من وجود فواتير تحميص مرتبطة بالموظف (مرحلة فقط)
			// إذا كان الموظف مربوط بفاتورة تحميص مرحلة، لا يمكن حذفه
			var hasRoastingInvoices = await roastingInvoiceRepository.HasRoastingInvoicesAsync(request.Id, cancellationToken);
			if (hasRoastingInvoices)
			{
				return new Error(ErrorCode.FieldDataInvalid, 
					translator.GetString("Cannot delete employee who has posted roasting invoices"), 
					nameof(request.Id));
			}

			employeeRepository.Delete(employee);
			await unitOfWork.SaveChangesAsync();

			return BaseResult.Ok();
		}
	}
}



