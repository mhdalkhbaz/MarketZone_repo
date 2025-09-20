using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Logistics.Enums;
using MarketZone.Domain.Inventory.Entities;
using System.Linq;

namespace MarketZone.Application.Features.Logistics.Commands.DistributionTrips.PostDistributionTrip
{
	public class PostDistributionTripCommandHandler : IRequestHandler<PostDistributionTripCommand, BaseResult>
	{
		private readonly IDistributionTripRepository _repository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IProductBalanceRepository _productBalanceRepository;

		public PostDistributionTripCommandHandler(
			IDistributionTripRepository repository,
			IUnitOfWork unitOfWork,
			IProductBalanceRepository productBalanceRepository)
		{
			_repository = repository;
			_unitOfWork = unitOfWork;
			_productBalanceRepository = productBalanceRepository;
		}

		public async Task<BaseResult> Handle(PostDistributionTripCommand request, CancellationToken cancellationToken)
		{
			try
			{
				var trip = await _repository.GetWithDetailsByIdAsync(request.Id, cancellationToken);
				if (trip is null)
					return new Error(ErrorCode.NotFound, "Distribution trip not found", nameof(request.Id));

				if (trip.Status == DistributionTripStatus.Posted)
					return BaseResult.Ok();

				// التحقق من أن الرحلة في حالة Draft
				if (trip.Status != DistributionTripStatus.Draft)
					return new Error(ErrorCode.FieldDataInvalid, "Cannot post trip that is not in Draft status", nameof(request.Id));

                // لا تأثير على المخزون في مرحلة Post حسب المتطلبات الجديدة

				// تغيير حالة الرحلة إلى Posted
				trip.SetStatus(DistributionTripStatus.Posted);

				await _unitOfWork.SaveChangesAsync();
				return BaseResult.Ok();
			}
			catch (System.Exception ex)
			{
				// في حالة الخطأ، التراجع عن جميع التغييرات
				await _unitOfWork.RollbackAsync();
				return new Error(ErrorCode.Exception, $"Error posting distribution trip: {ex.Message}", nameof(request.Id));
			}
		}
	}
}
