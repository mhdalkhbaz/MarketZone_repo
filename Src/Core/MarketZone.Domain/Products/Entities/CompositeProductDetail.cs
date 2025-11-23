using MarketZone.Domain.Common;

namespace MarketZone.Domain.Products.Entities
{
    public class CompositeProductDetail : AuditableBaseEntity
    {
        private CompositeProductDetail()
        {
        }

        public CompositeProductDetail(
            long compositeProductId,
            long componentProductId,
            decimal quantity)
        {
            CompositeProductId = compositeProductId;
            ComponentProductId = componentProductId;
            Quantity = quantity;
        }

        public CompositeProductDetail(
            long componentProductId,
            decimal quantity)
        {
            CompositeProductId = 0; // Will be set later
            ComponentProductId = componentProductId;
            Quantity = quantity;
        }

        public long CompositeProductId { get; private set; }
        public CompositeProduct CompositeProduct { get; private set; }
        public long ComponentProductId { get; private set; }
        public Product ComponentProduct { get; private set; }
        public decimal Quantity { get; private set; }

        public void Update(long componentProductId, decimal quantity)
        {
            ComponentProductId = componentProductId;
            Quantity = quantity;
        }

        public void SetCompositeProduct(CompositeProduct compositeProduct)
        {
            CompositeProduct = compositeProduct;
            CompositeProductId = compositeProduct.Id;
        }
    }
}

