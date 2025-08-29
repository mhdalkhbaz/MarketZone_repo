using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MarketZone.Application.Helpers;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Suppliers.DTOs;

namespace MarketZone.Application.Features.Suppliers.Queries.GetSupplierById
{
	public class GetSupplierByIdQueryHandler(ISupplierRepository supplierRepository, ITranslator translator, IMapper mapper) : IRequestHandler<GetSupplierByIdQuery, BaseResult<SupplierDto>>
	{
		public async Task<BaseResult<SupplierDto>> Handle(GetSupplierByIdQuery request, CancellationToken cancellationToken)
		{
			var supplier = await supplierRepository.GetByIdAsync(request.Id);

			if (supplier is null)
			{
				return new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.SupplierMessages.Supplier_NotFound_with_id(request.Id)), nameof(request.Id));
			}

			return mapper.Map<SupplierDto>(supplier);
		}
	}
}



