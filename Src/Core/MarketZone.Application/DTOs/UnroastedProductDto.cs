namespace MarketZone.Application.DTOs
{
    public class UnroastedProductDto
    {
        public string Value { get; set; }  // ID
        public string Label { get; set; }  // Product Name
        public decimal AvailableQty { get; set; }  // Available Quantity

        public UnroastedProductDto(string value, string label, decimal availableQty)
        {
            Value = value;
            Label = label;
            AvailableQty = availableQty;
        }
    }
}
