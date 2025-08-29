using MarketZone.Domain.Common;
using MarketZone.Domain.Products.Entities;

namespace MarketZone.Domain.Inventory.Entities
{
	public class UnroastedProdcutBalance : AuditableBaseEntity
	{
		private UnroastedProdcutBalance()
		{
		}

		public UnroastedProdcutBalance(long productId, decimal qty = 0)
		{
			ProductId = productId;
			Qty = qty;
			AvailableQty = qty;
		}

		public long ProductId { get; private set; }
		public Product Product { get; private set; }
		public decimal Qty { get; private set; }
		public decimal AvailableQty { get; private set; }

		public void Increase(decimal qty)
		{
			Qty += qty;
			AvailableQty += qty;
		}

		public void Decrease(decimal qty)
		{
			Qty -= qty;
			if (Qty < 0) Qty = 0;
		}

		public void Reserve(decimal qty)
		{
			if (AvailableQty >= qty)
			{
				AvailableQty -= qty;
			}
		}

		public void Release(decimal qty)
		{
			AvailableQty += qty;
			if (AvailableQty > Qty)
			{
				AvailableQty = Qty;
			}
		}

		public void Consume(decimal qty)
		{
			Decrease(qty);
			AvailableQty -= qty;
			if (AvailableQty < 0)
			{
				AvailableQty = 0;
			}
		}
	}
}


