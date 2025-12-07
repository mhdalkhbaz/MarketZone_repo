namespace MarketZone.Application.Parameters
{
    public class PurchaseInvoiceFilter : PaginationRequestParameter
    {
        public string InvoiceNumber { get; set; }
        public string SupplierName { get; set; }
        public string Name { get; set; } // Also searches in InvoiceNumber
        public string Notes { get; set; } // Maps to Notes
        public int? Status { get; set; } // Maps to PurchaseInvoiceStatus enum
    }
}


