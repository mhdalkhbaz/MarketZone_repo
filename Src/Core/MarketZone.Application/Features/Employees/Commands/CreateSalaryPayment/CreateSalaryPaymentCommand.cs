using System;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.Enums;
using MarketZone.Domain.Employees.Enums;

namespace MarketZone.Application.Features.Employees.Commands.CreateSalaryPayment
{
    public class CreateSalaryPaymentCommand : IRequest<BaseResult<long>>
    {
        public long EmployeeId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public long? CashRegisterId { get; set; }
        public Currency Currency { get; set; }
        public string Notes { get; set; }
        public decimal? Deduction { get; set; }  // الخصم
        public string DeductionNote { get; set; }  // ملاحظة سبب الخصم
        public SalaryType SalaryType { get; set; }
        public long? DistributionTripId { get; set; }
    }
}
