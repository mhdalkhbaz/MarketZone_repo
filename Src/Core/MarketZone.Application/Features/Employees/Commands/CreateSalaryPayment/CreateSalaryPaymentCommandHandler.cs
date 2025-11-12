using System;
using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Helpers;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Employees.Entities;
using MarketZone.Domain.Cash.Entities;

namespace MarketZone.Application.Features.Employees.Commands.CreateSalaryPayment
{
    public class CreateSalaryPaymentCommandHandler(
        IEmployeeRepository employeeRepository,
        IEmployeeSalaryRepository employeeSalaryRepository,
        ISalaryPaymentRepository salaryPaymentRepository,
        ICashRegisterRepository cashRegisterRepository,
        ICashTransactionRepository cashTransactionRepository,
        IUnitOfWork unitOfWork,
        ITranslator translator) : IRequestHandler<CreateSalaryPaymentCommand, BaseResult<long>>
    {
        public async Task<BaseResult<long>> Handle(CreateSalaryPaymentCommand request, CancellationToken cancellationToken)
        {
            // التحقق من وجود الموظف
            var employee = await employeeRepository.GetByIdAsync(request.EmployeeId);
            if (employee == null)
            {
                return new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.EmployeeMessages.Employee_NotFound_with_id(request.EmployeeId)), nameof(request.EmployeeId));
            }

            // التحقق من وجود الصندوق
            if (request.CashRegisterId.HasValue)
            {
                var cashRegister = await cashRegisterRepository.GetByIdAsync(request.CashRegisterId.Value);
                if (cashRegister == null)
                {
                    return new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.CashRegisterMessages.CashRegister_NotFound_with_id(request.CashRegisterId.Value)), nameof(request.CashRegisterId));
                }
            }

            // التحقق من صحة الشهر والسنة
            if (request.Month < 1 || request.Month > 12)
            {
                return new Error(ErrorCode.FieldDataInvalid, "Month must be between 1 and 12", nameof(request.Month));
            }

            if (request.Year < 2000 || request.Year > 3000)
            {
                return new Error(ErrorCode.FieldDataInvalid, "Year must be between 2000 and 3000", nameof(request.Year));
            }

            // إنشاء أو الحصول على EmployeeSalary
            var employeeSalary = await employeeSalaryRepository.GetOrCreateAsync(
                request.EmployeeId,
                request.Year,
                request.Month,
                employee.Salary,null);

            // إنشاء SalaryPayment
            var salaryPayment = new SalaryPayment(
                request.EmployeeId,
                request.Year,
                request.Month,
                request.Amount,
                request.PaymentDate,
                request.CashRegisterId,
                request.Notes ?? string.Empty,
                request.SalaryType,
                request.DistributionTripId);

            await salaryPaymentRepository.AddAsync(salaryPayment);

            // تحديث EmployeeSalary
            employeeSalary.AddPayment(request.Amount);
            employeeSalaryRepository.Update(employeeSalary);

            // إنشاء CashTransaction إذا كان هناك صندوق
            if (request.CashRegisterId.HasValue)
            {
                var cashTransaction = new CashTransaction(
                    request.CashRegisterId.Value,
                    Domain.Cash.Enums.TransactionType.Expense,
                    request.Amount,
                    request.PaymentDate,
                    Domain.Cash.Enums.ReferenceType.Salaries,
                    salaryPayment.Id,
                    $"Salary payment for employee {employee.FirstName} {employee.LastName} - {request.Year}/{request.Month}");

                await cashTransactionRepository.AddAsync(cashTransaction);

                // تحديث رصيد الصندوق
                var cashRegister = await cashRegisterRepository.GetByIdAsync(request.CashRegisterId.Value);
                cashRegister?.Adjust(-request.Amount);
                cashRegisterRepository.Update(cashRegister);
            }

            await unitOfWork.SaveChangesAsync();

            return BaseResult<long>.Ok(salaryPayment.Id);
        }
    }
}
