using System;
using MarketZone.Domain.Common;
using MarketZone.Domain.Products.Entities;

namespace MarketZone.Domain.Inventory.Entities
{
	public class InventoryHistory : AuditableBaseEntity
	{
#pragma warning disable
		private InventoryHistory()
		{
		}
#pragma warning restore
		public InventoryHistory(long productId, string transactionType, long? referenceId, decimal quantity, DateTime? date, string notes)
		{
			ProductId = productId;
			TransactionType = transactionType;
			ReferenceId = referenceId;
			Quantity = quantity;
			Date = date ?? DateTime.UtcNow;
			Notes = notes;
		}

		public long ProductId { get; private set; }
		public Product Product { get; private set; }
		public string TransactionType { get; private set; }
		public long? ReferenceId { get; private set; }
		public decimal Quantity { get; private set; }
		public DateTime Date { get; private set; }
		public string Notes { get; private set; }
	}
}



