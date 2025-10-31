using MarketZone.Domain.Common;
using MarketZone.Domain.Employees.Enums;
using System;

namespace MarketZone.Domain.Employees.Entities
{
    public class Employee : AuditableBaseEntity
    {
#pragma warning disable
        private Employee()
        {
        }
#pragma warning restore
        public Employee(string firstName, string lastName, string phone, string whatsAppPhone, string? email, string address, string jobTitle, decimal salary, DateTime hireDate, bool isActive = true, decimal? syrianMoney = null, decimal? dollarMoney = null, SalaryType salaryType = SalaryType.Fixed, decimal? salaryPercentage = null)
        {
            FirstName = firstName;
            LastName = lastName;
            Phone = phone;
            WhatsAppPhone = whatsAppPhone;
            Email = email;
            Address = address;
            JobTitle = jobTitle;
            Salary = salary;
            HireDate = hireDate;
            IsActive = isActive;
            SyrianMoney = syrianMoney;
            DollarMoney = dollarMoney;
            SalaryType = salaryType;
            SalaryPercentage = salaryPercentage;
        }

        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Phone { get; private set; }
        public string? WhatsAppPhone { get; private set; }
        public string? Email { get; private set; }
        public string Address { get; private set; }
        public string? JobTitle { get; private set; }
        public decimal Salary { get; private set; }
        public DateTime HireDate { get; private set; }
        public bool IsActive { get; private set; } = true;
        public decimal? SyrianMoney { get; private set; }
        public decimal? DollarMoney { get; private set; }
        public SalaryType SalaryType { get; private set; }
        public decimal? SalaryPercentage { get; private set; }

        public void Update(string firstName, string lastName, string phone, string whatsAppPhone, string? email, string address, string jobTitle, decimal salary, DateTime hireDate, bool isActive, decimal? syrianMoney = null, decimal? dollarMoney = null, SalaryType? salaryType = null, decimal? salaryPercentage = null)
        {
            FirstName = firstName;
            LastName = lastName;
            Phone = phone;
            WhatsAppPhone = whatsAppPhone;
            Email = email;
            Address = address;
            JobTitle = jobTitle;
            Salary = salary;
            HireDate = hireDate;
            IsActive = isActive;
            SyrianMoney = syrianMoney;
            DollarMoney = dollarMoney;
            if (salaryType.HasValue)
                SalaryType = salaryType.Value;
            SalaryPercentage = salaryPercentage;
        }
    }
}



