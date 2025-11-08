using MarketZone.Domain.Cash.Enums;

namespace MarketZone.Application.DTOs
{
	public class SelectListWithCurrencyDto
	{
	#pragma warning disable
		public SelectListWithCurrencyDto()
		{
		}
	#pragma warning restore
		public SelectListWithCurrencyDto(string label, string value, Currency currency)
		{
			Label = label;
			Value = value;
			Currency = currency;
		}

		public string Label { get; set; }
		public string Value { get; set; }
		public Currency Currency { get; set; }
	}
}


