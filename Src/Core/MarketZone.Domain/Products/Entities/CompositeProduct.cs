using MarketZone.Domain.Common;
using MarketZone.Domain.Products.Enums;
using System.Collections.Generic;

namespace MarketZone.Domain.Products.Entities
{
    public class CompositeProduct : AuditableBaseEntity
    {
        private CompositeProduct()
        {
            Details = new List<CompositeProductDetail>();
        }

        public CompositeProduct(
            long resultingProductId,
            decimal salePrice,
            decimal commissionPerKg)
        {
            ResultingProductId = resultingProductId;
            SalePrice = salePrice;
            CommissionPerKg = commissionPerKg;
            Status = CompositeProductStatus.Draft;
            Details = new List<CompositeProductDetail>();
        }

        public long ResultingProductId { get; private set; }
        public Product ResultingProduct { get; private set; }
        public decimal SalePrice { get; private set; }
        public decimal CommissionPerKg { get; private set; }
        public CompositeProductStatus Status { get; private set; } = CompositeProductStatus.Draft;

        public List<CompositeProductDetail> Details { get; private set; }

        public void Update(decimal salePrice, decimal commissionPerKg)
        {
            SalePrice = salePrice;
            CommissionPerKg = commissionPerKg;
        }

        public void SetStatus(CompositeProductStatus status)
        {
            Status = status;
        }

        public void AddDetail(CompositeProductDetail detail)
        {
            detail.SetCompositeProduct(this);
            Details.Add(detail);
        }

        public void RemoveDetail(CompositeProductDetail detail)
        {
            Details.Remove(detail);
        }
    }
}

