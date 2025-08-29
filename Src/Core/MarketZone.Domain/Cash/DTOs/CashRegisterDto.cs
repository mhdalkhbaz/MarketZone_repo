using System;
using MarketZone.Domain.Cash.Entities;

namespace MarketZone.Domain.Cash.DTOs
{
	public class CashRegisterDto
	{
		public CashRegisterDto()
		{
		}

		public CashRegisterDto(CashRegister e)
		{
			Id = e.Id;
			Name = e.Name;
			OpeningBalance = e.OpeningBalance;
			OpeningBalanceDollar = e.OpeningBalanceDollar;
			CurrentBalance = e.CurrentBalance;
			CurrentBalanceDollar = e.CurrentBalanceDollar;
			CreatedDateTime = e.Created;
		}

		public long Id { get; set; }
		public string Name { get; set; }
		public decimal OpeningBalance { get; set; }
		public decimal OpeningBalanceDollar { get; set; }
		public decimal CurrentBalance { get; set; }
		public decimal CurrentBalanceDollar { get; set; }
		public DateTime CreatedDateTime { get; set; }
	}
}


