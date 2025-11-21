using System;
using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Helpers;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Employees.Entities;
using MarketZone.Domain.Cash.Entities;
using MarketZone.Application.DTOs;

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


		// التحقق من صحة الشهر والسنة
		if (request.Month < 1 || request.Month > 12)
		{
			return new Error(ErrorCode.FieldDataInvalid, translator.GetString("Month_Must_Be_Between_1_And_12"), nameof(request.Month));
		}

		if (request.Year < 2000 || request.Year > 3000)
		{
			return new Error(ErrorCode.FieldDataInvalid, translator.GetString("Year_Must_Be_Between_2000_And_3000"), nameof(request.Year));
		}

            // إنشاء أو الحصول على EmployeeSalary
            var employeeSalary = await employeeSalaryRepository.GetOrCreateAsync(
                request.EmployeeId,
                request.Year,
                request.Month,
                employee.Salary,null);

		// التحقق من الخصم
		var deduction = request.Deduction ?? 0;
		if (deduction < 0)
		{
			return new Error(ErrorCode.FieldDataInvalid, 
				translator.GetString("Deduction_Cannot_Be_Negative"), 
				nameof(request.Deduction));
		}

            // حساب الخصم الإجمالي (الخصم الموجود + الخصم الجديد)
            var totalDeduction = employeeSalary.Deduction + deduction;

            // التحقق من أن المبلغ المدفوع + الخصم لا يتجاوز TotalSalary
            // TotalSalary = PaidAmount + Amount (المدفوع الجديد) + TotalDeduction
            var totalAfterPayment = employeeSalary.PaidAmount + request.Amount + totalDeduction;
		if (totalAfterPayment > employeeSalary.TotalSalary)
		{
			var message = translator.GetString(new TranslatorMessageDto("Total_Payment_And_Deduction_Exceed_Total_Salary", 
				new[] { 
					employeeSalary.TotalSalary.ToString(), 
					employeeSalary.PaidAmount.ToString(), 
					employeeSalary.Deduction.ToString(), 
					request.Amount.ToString(), 
					deduction.ToString(), 
					totalAfterPayment.ToString() 
				}));
			return new Error(ErrorCode.FieldDataInvalid, message, nameof(request.Amount));
		}

            // تطبيق الخصم إذا كان موجوداً (يضيف على الخصم الموجود)
            if (deduction > 0)
            {
                var newTotalDeduction = employeeSalary.Deduction + deduction;
                var combinedNote = string.IsNullOrEmpty(employeeSalary.Note) 
                    ? request.DeductionNote 
                    : string.IsNullOrEmpty(request.DeductionNote) 
                        ? employeeSalary.Note 
                        : $"{employeeSalary.Note}; {request.DeductionNote}";
                employeeSalary.SetDeduction(newTotalDeduction, combinedNote);
                employeeSalaryRepository.Update(employeeSalary);
            }

            // التحقق من رصيد الصندوق قبل الدفع
            if (request.CashRegisterId.HasValue)
            {
                var cashRegister = await cashRegisterRepository.GetByIdAsync(request.CashRegisterId.Value);
                if (cashRegister == null)
                {
                    return new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.CashRegisterMessages.CashRegister_NotFound_with_id(request.CashRegisterId.Value)), nameof(request.CashRegisterId));
                }

                // التحقق من أن الرصيد كافي حسب العملة
                if (request.Currency == Domain.Cash.Enums.Currency.SY)
                {
			if (cashRegister.CurrentBalance < request.Amount)
			{
				var message = translator.GetString(new TranslatorMessageDto("Insufficient_Balance_In_Cash_Register", 
					new[] { cashRegister.CurrentBalance.ToString(), request.Amount.ToString() }));
				return new Error(ErrorCode.FieldDataInvalid, message, nameof(request.Amount));
			}
			}
			else if (request.Currency == Domain.Cash.Enums.Currency.Dollar)
			{
				if (cashRegister.CurrentBalanceDollar < request.Amount)
				{
					var message = translator.GetString(new TranslatorMessageDto("Insufficient_Dollar_Balance_In_Cash_Register", 
						new[] { cashRegister.CurrentBalanceDollar.ToString(), request.Amount.ToString() }));
					return new Error(ErrorCode.FieldDataInvalid, message, nameof(request.Amount));
				}
			}
            }

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

            // تحديث EmployeeSalary - يزيد PaidAmount وينقص RemainingAmount تلقائياً
            employeeSalary.AddPayment(request.Amount);
            employeeSalaryRepository.Update(employeeSalary);

            // إنشاء CashTransaction إذا كان هناك صندوق
            if (request.CashRegisterId.HasValue)
            {
                var cashTransaction = new CashTransaction(
                    request.CashRegisterId.Value,
                    Domain.Cash.Enums.TransactionType.Expense,
                    request.Amount,
                    request.Currency,
                    request.PaymentDate,
                    Domain.Cash.Enums.ReferenceType.Salaries,
                    salaryPayment.Id,
                    $"Salary payment for employee {employee.FirstName} {employee.LastName} - {request.Year}/{request.Month}");

                await cashTransactionRepository.AddAsync(cashTransaction);

                // تحديث رصيد الصندوق حسب العملة
                var cashRegister = await cashRegisterRepository.GetByIdAsync(request.CashRegisterId.Value);
                if (cashRegister != null)
                {
                    if (request.Currency == Domain.Cash.Enums.Currency.SY)
                        cashRegister.Adjust(-request.Amount, null);
                    else
                        cashRegister.Adjust(0, -request.Amount);
                    cashRegisterRepository.Update(cashRegister);
                }
            }

            await unitOfWork.SaveChangesAsync();

            return BaseResult<long>.Ok(salaryPayment.Id);
        }
    }
}
