using MarketZone.Domain.Employees.Enums;
using System;

namespace MarketZone.Domain.Employees.DTOs
{
    public class SalaryPaymentDto
    {
        public long Id { get; set; }
        public long EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public long? CashRegisterId { get; set; }
        public string CashRegisterName { get; set; }
        public string Notes { get; set; }
        public SalaryType SalaryType { get; set; }
        public long? DistributionTripId { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime? LastModifiedDateTime { get; set; }
    }
}
