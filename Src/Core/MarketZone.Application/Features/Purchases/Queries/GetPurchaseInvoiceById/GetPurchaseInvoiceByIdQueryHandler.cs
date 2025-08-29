using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MarketZone.Application.Helpers;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Purchases.DTOs;

namespace MarketZone.Application.Features.Purchases.Queries.GetPurchaseInvoiceById
{
	public class GetPurchaseInvoiceByIdQueryHandler(IPurchaseInvoiceRepository repository, ITranslator translator, IMapper mapper) : IRequestHandler<GetPurchaseInvoiceByIdQuery, BaseResult<PurchaseInvoiceDto>>
	{
		public async Task<BaseResult<PurchaseInvoiceDto>> Handle(GetPurchaseInvoiceByIdQuery request, CancellationToken cancellationToken)
		{
			var entity = await repository.GetWithDetailsByIdAsync(request.Id, cancellationToken);
			if (entity is null)
			{
				return new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.PurchaseInvoiceMessages.PurchaseInvoice_NotFound_with_id(request.Id)), nameof(request.Id));
			}
			return mapper.Map<PurchaseInvoiceDto>(entity);
		}
	}
}



