using MarketZone.Domain.Common;

namespace MarketZone.Domain.Suppliers.Entities
{
    public class Supplier : AuditableBaseEntity
    {
        private Supplier()
        {
        }
        public Supplier(string name, string phone, string whatsAppPhone, string? email, string address, bool isActive = true)
        {
            Name = name;
            Phone = phone;
            WhatsAppPhone = whatsAppPhone;
            Email = email;
            Address = address;
            IsActive = isActive;
        }

        public string Name { get; private set; }
        public string Phone { get; private set; }
        public string? WhatsAppPhone { get; private set; }
        public string? Email { get; private set; }
        public string Address { get; private set; }
        public bool IsActive { get; private set; } = true;

        public void Update(string name, string phone, string whatsAppPhone, string? email, string address, bool isActive)
        {
            Name = name;
            Phone = phone;
            WhatsAppPhone = whatsAppPhone;
            Email = email;
            Address = address;
            IsActive = isActive;
        }
    }
}



