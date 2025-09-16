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
		public decimal TotalValue { get; private set; }

		public void Increase(decimal qty)
		{
			Qty += qty;
			AvailableQty += qty;
		}

		public void IncreaseWithValue(decimal qty, decimal value)
		{
			Qty += qty;
			AvailableQty += qty;
			TotalValue += value;
		}

		public void Decrease(decimal qty)
		{
			Qty -= qty;
			if (Qty < 0) Qty = 0;
		}

		public void DecreaseWithValue(decimal qty, decimal value)
		{
			Qty -= qty;
			if (Qty < 0) Qty = 0;
			TotalValue -= value;
			if (TotalValue < 0) TotalValue = 0;
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


