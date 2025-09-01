using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Logistics.Entities;

namespace MarketZone.Application.Features.Logistics.Commands.DistributionTrips.CreateDistributionTrip
{
	public class CreateDistributionTripCommandHandler(IDistributionTripRepository repository, IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<CreateDistributionTripCommand, BaseResult<long>>
	{
		public async Task<BaseResult<long>> Handle(CreateDistributionTripCommand request, CancellationToken cancellationToken)
		{
			// التحقق من وجود تفاصيل
			if (request.Details == null || !request.Details.Any())
				return new Error(ErrorCode.FieldDataInvalid, "At least one detail is required", nameof(request.Details));

			var trip = mapper.Map<DistributionTrip>(request);

			// إضافة التفاصيل
			if (request.Details?.Any() == true)
			{
				foreach (var d in request.Details)
				{
					var detail = new DistributionTripDetail(0, d.ProductId, d.Qty, d.ExpectedPrice);
					trip.AddDetail(detail);
				}
			}

			await repository.AddAsync(trip);
			await unitOfWork.SaveChangesAsync();
			return trip.Id;
		}
	}
}


