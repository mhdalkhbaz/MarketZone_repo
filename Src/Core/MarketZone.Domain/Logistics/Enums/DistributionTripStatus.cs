namespace MarketZone.Domain.Logistics.Enums
{
    public enum DistributionTripStatus : short
    {
        Draft = 0,              // مسودة
        Posted = 1,             // مرحلة (تم الترحيل)
        InProgress = 2,         // قيد التنفيذ
        GoodsReceived = 3,      // تم استلام البضاعة
        Completed = 4,          // مكتملة
        Cancelled = 5           // ملغية
    }
}
