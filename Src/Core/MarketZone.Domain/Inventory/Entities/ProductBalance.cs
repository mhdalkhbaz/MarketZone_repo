using MarketZone.Domain.Common;
using MarketZone.Domain.Products.Entities;

namespace MarketZone.Domain.Inventory.Entities
{
	public class ProductBalance : AuditableBaseEntity
	{
#pragma warning disable
		private ProductBalance()
		{
		}
#pragma warning restore
		public ProductBalance(long productId, decimal qty = 0, decimal availableQty = 0, decimal totalValue = 0)
		{
			ProductId = productId;
			Qty = qty;
			AvailableQty = availableQty;
			TotalValue = totalValue;
		}

		public long ProductId { get; private set; }
		public Product Product { get; private set; }
		public decimal Qty { get; private set; }
		public decimal AvailableQty { get; private set; }
		public decimal TotalValue { get; private set; }

		public void Adjust(decimal qtyDelta, decimal availableDelta)
		{
			Qty += qtyDelta;
			AvailableQty += availableDelta;
		}

		public void AdjustWithValue(decimal qtyDelta, decimal availableDelta, decimal valueDelta)
		{
			Qty += qtyDelta;
			AvailableQty += availableDelta;
			TotalValue += valueDelta;
		}
	}
}



