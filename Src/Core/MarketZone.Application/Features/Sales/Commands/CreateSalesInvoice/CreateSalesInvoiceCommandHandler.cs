using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Sales.Enums;

namespace MarketZone.Application.Features.Sales.Commands.CreateSalesInvoice
{
	public class CreateSalesInvoiceCommandHandler(
		ISalesInvoiceRepository repository,
		IDistributionTripRepository tripRepository,
		IUnitOfWork unitOfWork,
		IMapper mapper) : IRequestHandler<CreateSalesInvoiceCommand, BaseResult<long>>
	{
		public async Task<BaseResult<long>> Handle(CreateSalesInvoiceCommand request, CancellationToken cancellationToken)
		{
			// التحقق من وجود تفاصيل
			if (request.Details == null || !request.Details.Any())
				return new Error(ErrorCode.FieldDataInvalid, "At least one detail is required", nameof(request.Details));

			// التحقق من رحلة التوزيع إذا كانت محددة
			if (request.DistributionTripId.HasValue)
			{
				var trip = await tripRepository.GetByIdAsync(request.DistributionTripId.Value);
				if (trip is null)
					return new Error(ErrorCode.NotFound, "Distribution trip not found", nameof(request.DistributionTripId));

				// إذا كانت الفاتورة من نوع موزع، تأكد من أن الرحلة في الحالة المناسبة
				if (request.Type == SalesInvoiceType.Distributor && trip.Status != MarketZone.Domain.Logistics.Enums.DistributionTripStatus.GoodsReceived)
				{
					return new Error(ErrorCode.FieldDataInvalid, "Cannot create distributor invoice for trip that is not in GoodsReceived status", nameof(request.Type));
				}
			}

			var invoice = mapper.Map<MarketZone.Domain.Sales.Entities.SalesInvoice>(request);

			// ربط الفاتورة برحلة التوزيع إذا كانت محددة
			if (request.DistributionTripId.HasValue)
			{
				var trip = await tripRepository.GetByIdAsync(request.DistributionTripId.Value);
				if (trip != null)
				{
					invoice.SetDistributionTrip(trip);
					trip.AddSalesInvoice(invoice);
				}
			}

			// Save invoice first to get the ID
			await repository.AddAsync(invoice);
			await unitOfWork.SaveChangesAsync();

			// Add details after invoice is saved to get the correct ID
			if (request.Details?.Any() == true)
			{
				foreach (var d in request.Details)
				{
					var detail = mapper.Map<MarketZone.Domain.Sales.Entities.SalesInvoiceDetail>(d);
					invoice.AddDetail(detail);
				}
			}
			// Save changes to persist the details
			await unitOfWork.SaveChangesAsync();
			return invoice.Id;
		}
	}
}



