using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MarketZone.Application.Helpers;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Sales.DTOs;

namespace MarketZone.Application.Features.Sales.Queries.GetSalesInvoiceById
{
	public class GetSalesInvoiceByIdQueryHandler(ISalesInvoiceRepository repository, ITranslator translator, IMapper mapper) : IRequestHandler<GetSalesInvoiceByIdQuery, BaseResult<SalesInvoiceDto>>
	{
		public async Task<BaseResult<SalesInvoiceDto>> Handle(GetSalesInvoiceByIdQuery request, CancellationToken cancellationToken)
		{
			var entity = await repository.GetWithDetailsByIdAsync(request.Id, cancellationToken);
			if (entity is null)
			{
				return new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.SalesInvoiceMessages.SalesInvoice_NotFound_with_id(request.Id)), nameof(request.Id));
			}
			return mapper.Map<SalesInvoiceDto>(entity);
		}
	}
}



