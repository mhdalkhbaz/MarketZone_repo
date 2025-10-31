using MarketZone.Domain.Common;

namespace MarketZone.Domain.Employees.Entities
{
    public class EmployeeSalary : AuditableBaseEntity
    {
#pragma warning disable
        private EmployeeSalary()
        {
        }
#pragma warning restore

        public EmployeeSalary(long employeeId, int year, int month, decimal baseSalary, decimal? percentageAmount = null)
        {
            EmployeeId = employeeId;
            Year = year;
            Month = month;
            BaseSalary = baseSalary;
            PercentageAmount = percentageAmount ?? 0;
            PaidAmount = 0;
            CalculateTotalSalary();
        }

        public long EmployeeId { get; private set; }
        public Employee Employee { get; private set; }
        public int Year { get; private set; }
        public int Month { get; private set; }
        public decimal BaseSalary { get; private set; }  // الراتب الثابت
        public decimal PercentageAmount { get; private set; }  // المبلغ من النسبة
        public decimal PaidAmount { get; private set; }  // المبلغ المدفوع
        public decimal TotalSalary { get; private set; }  // إجمالي الراتب (ثابت + نسبة)
        public decimal RemainingAmount { get; private set; }  // المبلغ المتبقي

        /// <summary>
        /// تحديث المبلغ المدفوع عند دفع جزء من الراتب
        /// </summary>
        public void AddPayment(decimal amount)
        {
            if (amount <= 0)
                throw new System.ArgumentException("Amount must be greater than zero", nameof(amount));

            PaidAmount += amount;
            CalculateTotalSalary();
        }

        /// <summary>
        /// تحديث مبلغ النسبة من العمولة
        /// </summary>
        public void AddPercentageAmount(decimal amount)
        {
            if (amount < 0)
                throw new System.ArgumentException("Amount cannot be negative", nameof(amount));

            PercentageAmount += amount;
            CalculateTotalSalary();
        }

        /// <summary>
        /// تحديث الراتب الثابت
        /// </summary>
        public void UpdateBaseSalary(decimal baseSalary)
        {
            BaseSalary = baseSalary;
            CalculateTotalSalary();
        }

        /// <summary>
        /// حساب إجمالي الراتب والمبلغ المتبقي
        /// </summary>
        private void CalculateTotalSalary()
        {
            TotalSalary = BaseSalary + PercentageAmount;
            RemainingAmount = TotalSalary - PaidAmount;
        }
    }
}
