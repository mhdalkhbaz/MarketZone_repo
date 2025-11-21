using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Logistics.Enums;
using MarketZone.Domain.Inventory.Entities;
using MarketZone.Application.DTOs;

namespace MarketZone.Application.Features.Logistics.Commands.DistributionTrips.PostDistributionTrip
{
	public class PostDistributionTripCommandHandler : IRequestHandler<PostDistributionTripCommand, BaseResult>
	{
	private readonly IDistributionTripRepository _repository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly IProductBalanceRepository _productBalanceRepository;
	private readonly ITranslator _translator;

	public PostDistributionTripCommandHandler(
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

		public async Task<BaseResult> Handle(PostDistributionTripCommand request, CancellationToken cancellationToken)
		{
			try
			{
		var trip = await _repository.GetWithDetailsByIdAsync(request.Id, cancellationToken);
		if (trip is null)
			return new Error(ErrorCode.NotFound, _translator.GetString("DistributionTrip_NotFound"), nameof(request.Id));

		if (trip.Status == DistributionTripStatus.Posted)
			return BaseResult.Ok();

		// التحقق من أن الرحلة في حالة Draft
		if (trip.Status != DistributionTripStatus.Draft)
			return new Error(ErrorCode.FieldDataInvalid, _translator.GetString("Cannot_Post_Trip_Not_In_Draft"), nameof(request.Id));

				// إنقاص الكمية الأساسية (Qty) من ProductBalance عند Post الرحلة
				if (trip.Details != null && trip.Details.Any())
				{
					foreach (var detail in trip.Details)
					{
			var balance = await _productBalanceRepository.GetByProductIdAsync(detail.ProductId, cancellationToken);
			if (balance == null)
			{
				var message = _translator.GetString(new TranslatorMessageDto("Product_Balance_Not_Found_For_Product", new[] { detail.ProductId.ToString() }));
				return new Error(ErrorCode.FieldDataInvalid, message, nameof(request.Id));
			}

			// التحقق من أن الكمية الأساسية كافية
			if (balance.Qty < detail.Qty)
			{
				var message = _translator.GetString(new TranslatorMessageDto("Insufficient_Quantity_For_Product", 
					new[] { detail.ProductId.ToString(), balance.Qty.ToString(), detail.Qty.ToString() }));
				return new Error(ErrorCode.FieldDataInvalid, message, nameof(request.Id));
			}

						// إنقاص الكمية الأساسية (Qty) بقيمة الكمية المحملة
						balance.Adjust(-detail.Qty, 0);
						_productBalanceRepository.Update(balance);
					}
				}

				// تغيير حالة الرحلة إلى Posted
				trip.SetStatus(DistributionTripStatus.Posted);

				await _unitOfWork.SaveChangesAsync();
				return BaseResult.Ok();
			}
	catch (System.Exception ex)
	{
		// في حالة الخطأ، التراجع عن جميع التغييرات
		await _unitOfWork.RollbackAsync();
		var message = _translator.GetString(new TranslatorMessageDto("Error_Posting_Distribution_Trip", new[] { ex.Message }));
		return new Error(ErrorCode.Exception, message, nameof(request.Id));
	}
		}
	}
}
