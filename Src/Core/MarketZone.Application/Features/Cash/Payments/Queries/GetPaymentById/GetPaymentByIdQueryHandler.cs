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
        ISalesInvoiceRepository salesInvoiceRepository,
        IPurchaseInvoiceRepository purchaseInvoiceRepository,
        IRoastingInvoiceRepository roastingInvoiceRepository,
        ITranslator translator,
        IMapper mapper) : IRequestHandler<GetPaymentByIdQuery, BaseResult<PaymentDto>>
    {
        public async Task<BaseResult<PaymentDto>> Handle(GetPaymentByIdQuery request, CancellationToken cancellationToken)
        {
            // First, try to find in SalaryPayments table to ensure salary payments are handled correctly
            // This avoids ID collisions with the general Payment table (e.g., ID 2 being a supplier payment)
            var salaryPayment = await salaryPaymentRepository.GetByIdWithIncludesAsync(request.Id, cancellationToken);

            if (salaryPayment != null)
            {
                // Found in SalaryPayments, convert to PaymentDto
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
                    Status = PaymentStatus.Posted, 
                    IsIncome = false,
                    IsExpense = true,
                    User = salaryPayment.Employee != null 
                        ? new MarketZone.Domain.Cash.DTOs.SelectList(
                            salaryPayment.Employee.Id, 
                            $"{salaryPayment.Employee.FirstName} {salaryPayment.Employee.LastName}")
                        : null,
                    Deduction = salaryPayment.Deduction,
                    DeductionNote = salaryPayment.DeductionNote
                };

                return BaseResult<PaymentDto>.Ok(paymentDtoFromSalary);
            }

            // If not found in SalaryPayments, try to find in general Payment table
            var payment = await paymentRepository.GetByIdAsync(request.Id, cancellationToken);
            
            if (payment != null)
            {
                // Found in Payment table, convert to DTO using AutoMapper
                var paymentDto = mapper.Map<PaymentDto>(payment);
                
                if (payment.InvoiceId.HasValue && payment.InvoiceType.HasValue)
                {
                    // For invoice-related payments, get user (customer/supplier)
                    var res = await paymentRepository.GetUserForInviceByIdAsync(paymentDto.InvoiceId.Value, paymentDto.InvoiceType.Value, cancellationToken);
                    paymentDto.User = res;
                    
                    // Get invoice info
                    paymentDto.Invoice = await GetInvoiceInfo(paymentDto.InvoiceId.Value, paymentDto.InvoiceType.Value, cancellationToken);
                }
                
                return BaseResult<PaymentDto>.Ok(paymentDto);
            }

            return new Error(ErrorCode.NotFound, 
                translator.GetString("Payment_Not_Found"), 
                nameof(request.Id));
        }
        
        private async Task<SelectList> GetInvoiceInfo(long invoiceId, InvoiceType invoiceType, CancellationToken cancellationToken = default)
        {
            switch (invoiceType)
            {
                case InvoiceType.SalesInvoice:
                    var salesInvoice = await salesInvoiceRepository.GetByIdAsync(invoiceId);
                    return salesInvoice != null 
                        ? new SelectList(salesInvoice.Id, salesInvoice.InvoiceNumber) 
                        : null;
                        
                case InvoiceType.PurchaseInvoice:
                    var purchaseInvoice = await purchaseInvoiceRepository.GetByIdAsync(invoiceId);
                    return purchaseInvoice != null 
                        ? new SelectList(purchaseInvoice.Id, purchaseInvoice.InvoiceNumber) 
                        : null;
                        
                case InvoiceType.RoastingInvoice:
                    var roastingInvoice = await roastingInvoiceRepository.GetByIdAsync(invoiceId);
                    return roastingInvoice != null 
                        ? new SelectList(roastingInvoice.Id, roastingInvoice.InvoiceNumber) 
                        : null;
                        
                default:
                    return null;
            }
        }
    }
}