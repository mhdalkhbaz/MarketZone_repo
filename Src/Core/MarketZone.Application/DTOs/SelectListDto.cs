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

		public string Label { get; set; }
		public string Value { get; set; }
	}
}



