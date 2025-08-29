using System;
using MarketZone.Domain.Common;
using MarketZone.Domain.Products.Entities;

namespace MarketZone.Domain.Roasting.Entities
{
	public class RoastingOperation : AuditableBaseEntity
	{
		private RoastingOperation()
		{
		}

		public RoastingOperation(long productId, decimal quantityKg, decimal roastPricePerKg, DateTime? roastDate, string notes)
		{
			ProductId = productId;
			QuantityKg = quantityKg;
			RoastPricePerKg = roastPricePerKg;
			RoastDate = roastDate ?? DateTime.UtcNow;
			Notes = notes;
			TotalCost = roastPricePerKg * quantityKg;
		}

		public long ProductId { get; private set; }
		public Product Product { get; private set; }
		public decimal QuantityKg { get; private set; }
		public decimal RoastPricePerKg { get; private set; }
		public decimal TotalCost { get; private set; }
		public DateTime RoastDate { get; private set; }
		public string Notes { get; private set; }
	}
}


