using System;

namespace MarketZone.Domain.Cash.DTOs
{
    public class ExchangeRateDto
    {
        public long Id { get; set; }
        public decimal Rate { get; set; }              // 15000 ليرة سورية
        public DateTime EffectiveDate { get; set; }   // تاريخ السعر
        public bool IsActive { get; set; }            // السعر النشط
    }
}
