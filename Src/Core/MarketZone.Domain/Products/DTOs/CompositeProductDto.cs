using System;
using System.Collections.Generic;
using MarketZone.Domain.Products.Enums;

namespace MarketZone.Domain.Products.DTOs
{
    public class CompositeProductDto
    {
        public long Id { get; set; }
        public long ResultingProductId { get; set; }
        public string ResultingProductName { get; set; }
        public decimal SalePrice { get; set; }
        public decimal CommissionPerKg { get; set; }
        public CompositeProductStatus Status { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public List<CompositeProductDetailDto> Details { get; set; }
    }
}

