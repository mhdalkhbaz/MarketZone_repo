using System;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Roasting.Commands.CreateRoast
{
	public class CreateRoastCommand : IRequest<BaseResult<long>>
	{
		public long ProductId { get; set; }
		public decimal QuantityKg { get; set; }
		public decimal RoastPricePerKg { get; set; }
		public DateTime? RoastDate { get; set; }
		public string Notes { get; set; }
	}
}


