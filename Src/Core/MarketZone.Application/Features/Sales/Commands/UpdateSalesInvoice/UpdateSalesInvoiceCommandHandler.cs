using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MarketZone.Application.Helpers;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Sales.Commands.UpdateSalesInvoice
{
	public class UpdateSalesInvoiceCommandHandler(ISalesInvoiceRepository repository, IUnitOfWork unitOfWork, ITranslator translator, IMapper mapper) : IRequestHandler<UpdateSalesInvoiceCommand, BaseResult>
	{
		public async Task<BaseResult> Handle(UpdateSalesInvoiceCommand request, CancellationToken cancellationToken)
		{
			var entity = await repository.GetByIdAsync(request.Id);
			if (entity is null)
			{
				return new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.SalesInvoiceMessages.SalesInvoice_NotFound_with_id(request.Id)), nameof(request.Id));
			}
			mapper.Map(request, entity);
			await unitOfWork.SaveChangesAsync();
			return BaseResult.Ok();
		}
	}
}



