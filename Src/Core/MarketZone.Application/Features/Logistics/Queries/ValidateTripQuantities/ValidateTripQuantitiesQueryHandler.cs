using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Logistics.Queries.ValidateTripQuantities
{
	public class ValidateTripQuantitiesQueryHandler(IDistributionTripRepository tripRepository) : IRequestHandler<ValidateTripQuantitiesQuery, BaseResult<ValidateTripQuantitiesResult>>
	{
		public async Task<BaseResult<ValidateTripQuantitiesResult>> Handle(ValidateTripQuantitiesQuery request, CancellationToken cancellationToken)
		{
			var trip = await tripRepository.GetWithDetailsByIdAsync(request.TripId, cancellationToken);
			if (trip is null)
				return BaseResult<ValidateTripQuantitiesResult>.Failure();

			var result = new ValidateTripQuantitiesResult
			{
				IsValid = true,
				Errors = [],
				Warnings = []
			};

			foreach (var detail in trip.Details)
			{
				// الكمية المباعة تُحسب من فواتير المبيعات المرتبطة بالرحلة
				var soldQty = detail.SoldQty;
				var totalProcessed = soldQty + detail.ReturnedQty;
				
				// التحقق من أن الكميات المباعة والمرجعة لا تتجاوز الكمية المحملة
				if (totalProcessed > detail.Qty)
				{
					result.Errors.Add(new ValidationError
					{
						DetailId = detail.Id,
						ProductName = detail.Product?.Name ?? "غير محدد",
						ErrorMessage = $"الكمية المباعة ({soldQty}) + الكمية المرجعة ({detail.ReturnedQty}) = {totalProcessed} تتجاوز الكمية المحملة ({detail.Qty})"
					});
					result.IsValid = false;
				}

				// تحذير إذا كانت الكمية المرجعة كبيرة
				if (detail.ReturnedQty > detail.Qty * 0.3m) // أكثر من 30%
				{
					result.Warnings.Add(new ValidationWarning
					{
						DetailId = detail.Id,
						ProductName = detail.Product?.Name ?? "غير محدد",
						WarningMessage = $"الكمية المرجعة ({detail.ReturnedQty}) تشكل نسبة عالية من الكمية المحملة ({detail.Qty})"
					});
				}

				// تحذير إذا لم يتم تسجيل أي مبيعات أو مرتجعات
				if (soldQty == 0 && detail.ReturnedQty == 0)
				{
					result.Warnings.Add(new ValidationWarning
					{
						DetailId = detail.Id,
						ProductName = detail.Product?.Name ?? "غير محدد",
						WarningMessage = "لم يتم تسجيل أي مبيعات أو مرتجعات لهذا المنتج"
					});
				}

				// تحذير إذا لم يتم إنشاء أي فواتير مبيعات
				if (soldQty == 0 && trip.DistributionTripSalesInvoices?.Any() == true)
				{
					result.Warnings.Add(new ValidationWarning
					{
						DetailId = detail.Id,
						ProductName = detail.Product?.Name ?? "غير محدد",
						WarningMessage = "تم إنشاء فواتير مبيعات ولكن لم يتم بيع هذا المنتج"
					});
				}
			}

			return result;
		}
	}
}
