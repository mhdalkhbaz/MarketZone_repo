namespace MarketZone.Domain.Roasting.DTOs
{
    public class RoastingInvoiceUnpaidDto
    {
        public long Id { get; set; }
        public string InvoiceNumber { get; set; }
        public decimal UnpaidAmount { get; set; }
    }
}

