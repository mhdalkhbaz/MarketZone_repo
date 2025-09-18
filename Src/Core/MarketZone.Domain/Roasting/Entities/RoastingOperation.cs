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

		public RoastingOperation(long rawProductId, long roastedProductId, decimal quantityKg, decimal roastPricePerKg, DateTime? roastDate, string notes)
		{
			RawProductId = rawProductId;
			RoastedProductId = roastedProductId;
			QuantityKg = quantityKg;
			RoastPricePerKg = roastPricePerKg;
			RoastDate = roastDate ?? DateTime.UtcNow;
			Notes = notes;
			TotalCost = roastPricePerKg * quantityKg;
		}

		public long RawProductId { get; private set; }
		public Product RawProduct { get; private set; }
		public long RoastedProductId { get; private set; }
		public Product RoastedProduct { get; private set; }
		public decimal QuantityKg { get; private set; }
		public decimal RoastPricePerKg { get; private set; }
		public decimal TotalCost { get; private set; }
		public DateTime RoastDate { get; private set; }
		public string Notes { get; private set; }
	}
}


