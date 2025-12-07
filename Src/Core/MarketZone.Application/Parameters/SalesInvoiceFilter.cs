namespace MarketZone.Application.Parameters
{
    public class SalesInvoiceFilter : PaginationRequestParameter
    {
        public string InvoiceNumber { get; set; }
        public string CustomerName { get; set; }
        public string Name { get; set; } // Also searches in InvoiceNumber
        public string Notes { get; set; } // Maps to Notes
        public int? Status { get; set; } // Maps to SalesInvoiceStatus enum
        public int? Type { get; set; } // Maps to SalesInvoiceType enum
    }
}


