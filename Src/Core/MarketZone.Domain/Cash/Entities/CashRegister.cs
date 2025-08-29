using MarketZone.Domain.Common;

namespace MarketZone.Domain.Cash.Entities
{
	public class CashRegister : AuditableBaseEntity
	{
		private CashRegister()
		{
		}

		public CashRegister(string name, decimal openingBalance = 0, decimal openingBalanceDollar = 0)
		{
			Name = name;
			OpeningBalance = openingBalance;
			OpeningBalanceDollar = openingBalanceDollar;
			CurrentBalance = openingBalance;
			CurrentBalanceDollar = openingBalanceDollar;
		}

		public string Name { get; private set; }
		public decimal OpeningBalance { get; private set; }
		public decimal OpeningBalanceDollar { get; private set; }
		public decimal CurrentBalance { get; private set; }
		public decimal CurrentBalanceDollar { get; private set; }

		public void Adjust(decimal amount, decimal? amountDollar = null)
		{
			CurrentBalance += amount;
			if (amountDollar.HasValue)
				CurrentBalanceDollar += amountDollar.Value;
		}
	}
}


