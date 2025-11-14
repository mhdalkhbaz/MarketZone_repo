using System;

namespace MarketZone.Domain.Employees.DTOs
{
    public class EmployeeSalaryDto
    {
        public long Id { get; set; }
        public long EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal BaseSalary { get; set; }
        public decimal PercentageAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal Deduction { get; set; }
        public string Note { get; set; }
        public decimal TotalSalary { get; set; }
        public decimal RemainingAmount { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime? LastModifiedDateTime { get; set; }
    }
}
