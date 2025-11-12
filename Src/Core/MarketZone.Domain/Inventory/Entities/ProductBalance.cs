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
		public ProductBalance(long productId, decimal qty = 0, decimal availableQty = 0, decimal totalValue = 0, decimal salePrice = 0)
		{
			ProductId = productId;
			Qty = qty;
			AvailableQty = availableQty;
			TotalValue = totalValue;
			AverageCost = Qty > 0 ? decimal.Round(TotalValue / Qty, 6) : 0;
			SalePrice = salePrice;
		}

		public long ProductId { get; private set; }
		public Product Product { get; private set; }
		public decimal Qty { get; private set; }
		public decimal AvailableQty { get; private set; }
		public decimal TotalValue { get; private set; }
		public decimal AverageCost { get; private set; }
		public decimal SalePrice { get; private set; }

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
			AverageCost = Qty > 0 ? decimal.Round(TotalValue / Qty, 6) : 0;
		}

		public void AdjustValue(decimal valueDelta)
		{
			TotalValue += valueDelta;
			AverageCost = Qty > 0 ? decimal.Round(TotalValue / Qty, 6) : 0;
		}

		public void SetAverageCost(decimal averageCost)
		{
			AverageCost = averageCost;
			TotalValue = Qty > 0 ? decimal.Round(AverageCost * Qty, 6) : 0;
		}

		public void SetSalePrice(decimal salePrice)
		{
			SalePrice = salePrice < 0 ? 0 : salePrice;
		}
	}
}



