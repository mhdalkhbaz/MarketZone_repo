namespace MarketZone.Application.Parameters
{
    public class RoastingInvoiceFilter : PaginationRequestParameter
    {
        public string InvoiceNumber { get; set; }
        public string Name { get; set; } // Also searches in InvoiceNumber
        public string Description { get; set; } // Maps to Notes
        public int? Status { get; set; } // Maps to RoastingInvoiceStatus enum
    }
}


