namespace MarketZone.Application.DTOs
{
    public class ProductSelectListDto
    {
        public string Value { get; set; }  // Product ID
        public string Label { get; set; }  // Product resultingProductName
        public decimal Qty { get; set; }   // Available Quantity (Remaining for trip or Available for sale)
		public decimal? SalePrice { get; set; } // Product sale price (optional)

		public ProductSelectListDto(string value, string label, decimal qty, decimal? salePrice = null)
        {
            Value = value;
            Label = label;
            Qty = qty;
			SalePrice = salePrice;
        }
    }
}
