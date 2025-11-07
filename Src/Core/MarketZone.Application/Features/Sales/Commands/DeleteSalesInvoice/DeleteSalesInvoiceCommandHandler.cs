using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Helpers;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Sales.Enums;

namespace MarketZone.Application.Features.Sales.Commands.DeleteSalesInvoice
{
	public class DeleteSalesInvoiceCommandHandler(ISalesInvoiceRepository repository, IUnitOfWork unitOfWork, ITranslator translator) : IRequestHandler<DeleteSalesInvoiceCommand, BaseResult>
	{
		public async Task<BaseResult> Handle(DeleteSalesInvoiceCommand request, CancellationToken cancellationToken)
		{
			var entity = await repository.GetByIdAsync(request.Id);
			if (entity is null)
			{
				return new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.SalesInvoiceMessages.SalesInvoice_NotFound_with_id(request.Id)), nameof(request.Id));
			}
			if (entity.Status == SalesInvoiceStatus.Posted)
			{
				return new Error(ErrorCode.AccessDenied, translator.GetString("SalesInvoice_Delete_NotAllowed_After_Post"), nameof(request.Id));
			}
			repository.Delete(entity);
			await unitOfWork.SaveChangesAsync();
			return BaseResult.Ok();
		}
	}
}



