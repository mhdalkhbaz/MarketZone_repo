using AutoMapper;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Sales.Enums;
using MarketZone.Application.Interfaces.Services;
using System.Threading;
using System.Threading.Tasks;

namespace MarketZone.Application.Features.Sales.Commands.PostSalesInvoice
{
    public class PostSalesInvoiceCommandHandler(ISalesInvoiceRepository repository, IUnitOfWork unitOfWork, ITranslator translator, IMapper mapper, ISalesInventoryService inventoryService) : IRequestHandler<PostSalesInvoiceCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(PostSalesInvoiceCommand request, CancellationToken cancellationToken)
        {
            var invoice = await repository.GetByIdAsync(request.Id);
            if (invoice is null)
                return new Error(ErrorCode.NotFound, translator.GetString("SalesInvoice_NotFound_with_id"), nameof(request.Id));
            if (invoice.Status == SalesInvoiceStatus.Posted)
                return BaseResult.Ok();

            await inventoryService.ApplyOnPostAsync(invoice, cancellationToken);
            invoice.SetStatus(SalesInvoiceStatus.Posted);
            await unitOfWork.SaveChangesAsync();
            return BaseResult.Ok();
        }
    }
}


