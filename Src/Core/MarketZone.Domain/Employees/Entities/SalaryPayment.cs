using MarketZone.Domain.Common;
using MarketZone.Domain.Employees.Enums;
using System;

namespace MarketZone.Domain.Employees.Entities
{
    public class SalaryPayment : AuditableBaseEntity
    {
#pragma warning disable
        private SalaryPayment()
        {
        }
#pragma warning restore

        public SalaryPayment(long employeeId, int year, int month, decimal amount, DateTime paymentDate, long? cashRegisterId, string notes, SalaryType salaryType, long? distributionTripId = null)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero", nameof(amount));

            EmployeeId = employeeId;
            Year = year;
            Month = month;
            Amount = amount;
            PaymentDate = paymentDate;
            CashRegisterId = cashRegisterId;
            Notes = notes;
            SalaryType = salaryType;
            DistributionTripId = distributionTripId;
        }

        public long EmployeeId { get; private set; }
        public Employee Employee { get; private set; }
        public int Year { get; private set; }
        public int Month { get; private set; }
        public decimal Amount { get; private set; }
        public DateTime PaymentDate { get; private set; }
        public long? CashRegisterId { get; private set; }
        public MarketZone.Domain.Cash.Entities.CashRegister CashRegister { get; private set; }
        public string Notes { get; private set; }
        public SalaryType SalaryType { get; private set; }
        public long? DistributionTripId { get; private set; }  // ربط برحلة التوزيع إذا كانت النسبة من عمولة
        public MarketZone.Domain.Logistics.Entities.DistributionTrip DistributionTrip { get; private set; }

        public void UpdateAmount(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero", nameof(amount));

            Amount = amount;
        }

        public void UpdateNotes(string notes)
        {
            Notes = notes;
        }
    }
}
