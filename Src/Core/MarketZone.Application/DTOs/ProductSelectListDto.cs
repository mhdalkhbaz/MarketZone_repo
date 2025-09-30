namespace MarketZone.Application.DTOs
{
    public class ProductSelectListDto
    {
        public string Value { get; set; }  // Product ID
        public string Label { get; set; }  // Product Name
        public decimal Qty { get; set; }   // Available Quantity (Remaining for trip or Available for sale)

        public ProductSelectListDto(string value, string label, decimal qty)
        {
            Value = value;
            Label = label;
            Qty = qty;
        }
    }
}
