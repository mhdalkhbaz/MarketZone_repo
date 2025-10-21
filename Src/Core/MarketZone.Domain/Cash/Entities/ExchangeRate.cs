using System;
using MarketZone.Domain.Common;

namespace MarketZone.Domain.Cash.Entities
{
    public class ExchangeRate : AuditableBaseEntity
    {
        private ExchangeRate()
        {
        }

        public ExchangeRate(decimal rate, DateTime effectiveDate)
        {
            Rate = rate;                    // 15000 ليرة سورية
            EffectiveDate = effectiveDate;   // تاريخ السعر
            IsActive = true;               // السعر النشط
        }

        public decimal Rate { get; private set; }           // 15000 ليرة سورية لكل دولار
        public DateTime EffectiveDate { get; private set; } // تاريخ السعر
        public bool IsActive { get; private set; }         // السعر النشط

        public void Deactivate() => IsActive = false;
        public void UpdateRate(decimal newRate) => Rate = newRate;
    }
}