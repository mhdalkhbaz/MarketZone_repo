using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.DTOs;
using MarketZone.Domain.Cash.Entities;
using MarketZone.Domain.Cash.Enums;

namespace MarketZone.Application.Features.Cash.Payments.Queries.GetPaymentById
{
    public class GetPaymentByIdQueryHandler(
        IPaymentRepository paymentRepository,
        ISalaryPaymentRepository salaryPaymentRepository,
        ICashTransactionRepository cashTransactionRepository,
        ITranslator translator,
        IMapper mapper) : IRequestHandler<GetPaymentByIdQuery, BaseResult<PaymentDto>>
    {
        public async Task<BaseResult<PaymentDto>> Handle(GetPaymentByIdQuery request, CancellationToken cancellationToken)
        {
            // First, try to find in Payment table
            var payment = await paymentRepository.GetByIdAsync(request.Id, cancellationToken);
            
            if (payment != null)
            {
                // Found in Payment table, convert to DTO using AutoMapper
                var paymentDto = mapper.Map<PaymentDto>(payment);
                var res = await paymentRepository.GetUserForInviceByIdAsync(paymentDto.InvoiceId.Value, paymentDto.InvoiceType.Value, cancellationToken);
                paymentDto.User = res;
                return BaseResult<PaymentDto>.Ok(paymentDto);
            }

            // If not found in Payment table, try to find in SalaryPayments table
            var salaryPayment = await salaryPaymentRepository.GetByIdWithIncludesAsync(request.Id, cancellationToken);

            if (salaryPayment == null)
            {
                return new Error(ErrorCode.NotFound, 
                    translator.GetString("Payment_Not_Found"), 
                    nameof(request.Id));
            }

            // Found in SalaryPayments, convert to PaymentDto
            // Get Currency from CashTransaction if exists, otherwise from Employee or default
            var currency = Currency.SY;
            var cashTransaction = await cashTransactionRepository.GetByReferenceAsync(ReferenceType.Salaries, salaryPayment.Id, cancellationToken);
            
            if (cashTransaction != null)
            {
                currency = cashTransaction.Currency;
            }
            else if (salaryPayment.Employee?.Currency.HasValue == true)
            {
                currency = salaryPayment.Employee.Currency.Value;
            }

            var paymentDtoFromSalary = new PaymentDto
            {
                Id = salaryPayment.Id,
                CashRegisterId = salaryPayment.CashRegisterId,
                PaymentType = PaymentType.SalaryPayment,
                InvoiceId = null,
                InvoiceType = null,
                PaymentDate = salaryPayment.PaymentDate,
                Amount = salaryPayment.Amount,
                Currency = currency,
                PaymentCurrency = currency,
                ExchangeRate = null,
                AmountInPaymentCurrency = salaryPayment.Amount,
                Notes = salaryPayment.Notes,
                Description = $"Salary payment for {salaryPayment.Employee?.FirstName} {salaryPayment.Employee?.LastName} - {salaryPayment.Year}/{salaryPayment.Month}",
                ReceivedBy = null,
                PaidBy = salaryPayment.Employee != null ? $"{salaryPayment.Employee.FirstName} {salaryPayment.Employee.LastName}" : null,
                IsConfirmed = true,
                Status = PaymentStatus.Posted, // SalaryPayment is always posted
                IsIncome = false,
                IsExpense = true
            };

         
            return BaseResult<PaymentDto>.Ok(paymentDtoFromSalary);
        }
    }
}

    