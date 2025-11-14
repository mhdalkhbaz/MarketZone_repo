namespace MarketZone.Application.DTOs
{
    public class UnroastedProductDto
    {
        public string Value { get; set; }  // ID
        public string Label { get; set; }  // Product Name
        public decimal AvailableQty { get; set; }  // Available Quantity
        public decimal AverageCost { get; set; }  // Sale price per unit (stored in AverageCost field)
        public decimal SalePrice { get; set; }  // Sale price per unit (stored in AverageCost field)

        public UnroastedProductDto(string value, string label, decimal availableQty)
        {
            Value = value;
            Label = label;
            AvailableQty = availableQty;
        }

        public UnroastedProductDto(string value, string label, decimal availableQty, decimal averageCost, decimal salePrice)
        {
            Value = value;
            Label = label;
            AvailableQty = availableQty;
            AverageCost = averageCost;
            SalePrice = salePrice;
        }
    }
}
