using MarketZone.Domain.Common;
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
        public Employee(string firstName, string lastName, string phone, string whatsAppPhone, string? email, string address, string jobTitle, decimal salary, DateTime hireDate, bool isActive = true)
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
        }

        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Phone { get; private set; }
        public string? WhatsAppPhone { get; private set; }
        public string? Email { get; private set; }
        public string Address { get; private set; }
        public string JobTitle { get; private set; }
        public decimal Salary { get; private set; }
        public DateTime HireDate { get; private set; }
        public bool IsActive { get; private set; } = true;

        public void Update(string firstName, string lastName, string phone, string whatsAppPhone, string? email, string address, string jobTitle, decimal salary, DateTime hireDate, bool isActive)
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
        }
    }
}



