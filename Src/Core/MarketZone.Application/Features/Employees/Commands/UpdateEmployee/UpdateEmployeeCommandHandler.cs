using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MarketZone.Application.Helpers;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Employees.Commands.UpdateEmployee
{
	public class UpdateEmployeeCommandHandler(
		IEmployeeRepository employeeRepository, 
		IUnitOfWork unitOfWork, 
		ITranslator translator, 
		IMapper mapper,
		IRoastingInvoiceRepository roastingInvoiceRepository) : IRequestHandler<UpdateEmployeeCommand, BaseResult>
	{
		public async Task<BaseResult> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
		{
			var employee = await employeeRepository.GetByIdAsync(request.Id);

			if (employee is null)
			{
				return new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.EmployeeMessages.Employee_NotFound_with_id(request.Id)), nameof(request.Id));
			}

			// التحقق من وجود فواتير تحميص مرتبطة بالموظف (مرحلة فقط)
			// إذا كان الموظف مربوط بفاتورة تحميص مرحلة، لا يمكن تعديله
			var hasRoastingInvoices = await roastingInvoiceRepository.HasRoastingInvoicesAsync(request.Id, cancellationToken);
			if (hasRoastingInvoices)
			{
				return new Error(ErrorCode.FieldDataInvalid, 
					translator.GetString("Cannot_Update_Employee_With_Posted_Roasting_Invoices"), 
					nameof(request.Id));
			}

			employee.Update(
				request.FirstName,
				request.LastName,
				request.Phone,
				request.WhatsAppPhone,
				request.Email,
				request.Address,
				request.JobTitle,
				request.Salary ?? employee.Salary,
				request.HireDate ?? employee.HireDate,
				request.IsActive ?? employee.IsActive,
				request.SyrianMoney,
				request.DollarMoney,
				request.SalaryType,
				request.SalaryPercentage,
				request.Currency
			);
			await unitOfWork.SaveChangesAsync();

			return BaseResult.Ok();
		}
	}
}



