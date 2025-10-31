using MarketZone.Domain.Common;
using MarketZone.Domain.Cash.Enums;

namespace MarketZone.Domain.Customers.Entities
{
    public class Customer : AuditableBaseEntity
    {
#pragma warning disable
        private Customer()
        {
        }
#pragma warning restore
        public Customer(string name, string phone, string whatsAppPhone, string? email, string address, Currency? currency = null, bool isActive = true)
        {
            Name = name;
            Phone = phone;
            WhatsAppPhone = whatsAppPhone;
            Email = email;
            Address = address;
            Currency = currency;
            IsActive = isActive;
        }

        public string Name { get; private set; }
        public string Phone { get; private set; }
        public string? WhatsAppPhone { get; private set; }
        public string? Email { get; private set; }
        public string? Address { get; private set; }
        public Currency? Currency { get; private set; }
        public bool IsActive { get; private set; } = true;

        public void Update(string name, string phone, string whatsAppPhone, string? email, string address, Currency? currency, bool isActive)
        {
            Name = name;
            Phone = phone;
            WhatsAppPhone = whatsAppPhone;
            Email = email;
            Address = address;
            Currency = currency;
            IsActive = isActive;
        }
    }
}



