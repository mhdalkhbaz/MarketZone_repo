using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Helpers;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Sales.Enums;

namespace MarketZone.Application.Features.Sales.Commands.UpdateSalesInvoice
{
	public class UpdateSalesInvoiceCommandHandler(ISalesInvoiceRepository repository, IUnitOfWork unitOfWork, ITranslator translator) : IRequestHandler<UpdateSalesInvoiceCommand, BaseResult>
	{
		public async Task<BaseResult> Handle(UpdateSalesInvoiceCommand request, CancellationToken cancellationToken)
		{
			var entity = await repository.GetByIdAsync(request.Id);
			if (entity is null)
			{
				return new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.SalesInvoiceMessages.SalesInvoice_NotFound_with_id(request.Id)), nameof(request.Id));
			}
			
			if (entity.Status == SalesInvoiceStatus.Posted)
				return new Error(ErrorCode.AccessDenied, translator.GetString("SalesInvoice_Update_NotAllowed_After_Post"), nameof(request.Id));
			
			entity.Update(
				request.InvoiceNumber ?? entity.InvoiceNumber,
				request.CustomerId ?? entity.CustomerId,
				request.InvoiceDate ?? entity.InvoiceDate,
				request.TotalAmount ?? entity.TotalAmount,
				request.Discount ?? entity.Discount,
				request.PaymentMethod ?? entity.PaymentMethod,
				request.Notes ?? entity.Notes,
				request.Currency ?? entity.Currency
			);
			
			await unitOfWork.SaveChangesAsync();
			return BaseResult.Ok();
		}
	}
}



