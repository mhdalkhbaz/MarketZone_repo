using AutoMapper;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Services;
using MarketZone.Application.Wrappers;
using System.Threading;
using System.Threading.Tasks;

namespace MarketZone.Application.Features.Roasting.Commands.CreateRoast
{
	public class CreateRoastCommandHandler(IRoastingService roastingService, IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<CreateRoastCommand, BaseResult<long>>
	{
		public async Task<BaseResult<long>> Handle(CreateRoastCommand request, CancellationToken cancellationToken)
		{
			var id = await roastingService.RoastAsync(request.ProductId, request.QuantityKg, request.RoastPricePerKg, request.RoastDate, request.Notes, cancellationToken);
			await unitOfWork.SaveChangesAsync();
			return id;
		}
	}
}


