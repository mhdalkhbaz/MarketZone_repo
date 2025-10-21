using MarketZone.Domain.Common;

namespace MarketZone.Domain.Suppliers.Entities
{
    public class Supplier : AuditableBaseEntity
    {
        private Supplier()
        {
        }
        public Supplier(string name, string phone, string whatsAppPhone, string? email, string address, string? currency = null, bool isActive = true)
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
        public string Address { get; private set; }
        public string? Currency { get; private set; }
        public bool IsActive { get; private set; } = true;

        public void Update(string name, string phone, string whatsAppPhone, string? email, string address, string? currency, bool isActive)
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



