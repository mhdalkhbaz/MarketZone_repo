using MarketZone.Domain.Cash.Enums;

namespace MarketZone.Application.DTOs
{
	public class SelectListDto
	{
#pragma warning disable
		public SelectListDto()
		{
		}
#pragma warning restore
		public SelectListDto(string label, string value)
		{
			Label = label;
			Value = value;
		}

		public SelectListDto(string label, string value, Currency? currency)
		{
			Label = label;
			Value = value;
			Currency = currency;
		}

		public string Label { get; set; }
		public string Value { get; set; }
		public Currency? Currency { get; set; }
	}
}



