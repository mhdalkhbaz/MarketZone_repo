using System.Collections.Generic;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Logistics.Queries.ValidateTripQuantities
{
	public class ValidateTripQuantitiesQuery : IRequest<BaseResult<ValidateTripQuantitiesResult>>
	{
		public long TripId { get; set; }
	}

	public class ValidateTripQuantitiesResult
	{
		public bool IsValid { get; set; }
		public List<ValidationError> Errors { get; set; } = new List<ValidationError>();
		public List<ValidationWarning> Warnings { get; set; } = new List<ValidationWarning>();
	}

	public class ValidationError
	{
		public long DetailId { get; set; }
		public string ProductName { get; set; } = string.Empty;
		public string ErrorMessage { get; set; } = string.Empty;
	}

	public class ValidationWarning
	{
		public long DetailId { get; set; }
		public string ProductName { get; set; } = string.Empty;
		public string WarningMessage { get; set; } = string.Empty;
	}
}
