using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Logistics.Enums;

namespace MarketZone.Application.Features.Logistics.Commands.DistributionTrips.DeleteDistributionTrip
{
	public class DeleteDistributionTripCommandHandler : IRequestHandler<DeleteDistributionTripCommand, BaseResult>
	{
		private readonly IDistributionTripRepository _repository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IProductBalanceRepository _productBalanceRepository;
		private readonly ITranslator _translator;

		public DeleteDistributionTripCommandHandler(
			IDistributionTripRepository repository,
			IUnitOfWork unitOfWork,
			IProductBalanceRepository productBalanceRepository,
			ITranslator translator)
		{
			_repository = repository;
			_unitOfWork = unitOfWork;
			_productBalanceRepository = productBalanceRepository;
			_translator = translator;
		}

		public async Task<BaseResult> Handle(DeleteDistributionTripCommand request, CancellationToken cancellationToken)
		{
			try
			{
				// الحصول على رحلة التوزيع مع التفاصيل
				var trip = await _repository.GetWithDetailsByIdAsync(request.TripId, cancellationToken);
				if (trip == null)
					return new Error(ErrorCode.NotFound, "Distribution trip not found", nameof(request.TripId));

				// التحقق من أن الرحلة في حالة مسودة (Draft) فقط
				if (trip.Status != DistributionTripStatus.Draft)
					return new Error(ErrorCode.FieldDataInvalid, "Cannot delete distribution trip that is not in Draft status. Only Draft trips can be deleted.", nameof(request.TripId));

				// التحقق من عدم وجود فواتير مبيعات مرتبطة
				if (trip.DistributionTripSalesInvoices != null && trip.DistributionTripSalesInvoices.Any())
					return new Error(ErrorCode.FieldDataInvalid, "Cannot delete distribution trip that has associated sales invoices", nameof(request.TripId));

				// التحقق من عدم وجود كميات مباعة أو مرجعة
				var hasSoldOrReturnedQuantities = trip.Details.Any(d => d.SoldQty > 0 || d.ReturnedQty > 0);
				if (hasSoldOrReturnedQuantities)
					return new Error(ErrorCode.FieldDataInvalid, "Cannot delete distribution trip that has sold or returned quantities", nameof(request.TripId));

				// حذف رحلة التوزيع
				_repository.Delete(trip);
				await _unitOfWork.SaveChangesAsync();

				return BaseResult.Ok();
			}
			catch (System.Exception ex)
			{
				// في حالة الخطأ، التراجع عن جميع التغييرات
				await _unitOfWork.RollbackAsync();
				return new Error(ErrorCode.Exception, $"Error deleting distribution trip: {ex.Message}", nameof(request.TripId));
			}
		}
	}
}

