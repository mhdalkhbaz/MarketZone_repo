using AutoMapper;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Interfaces.Services;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Purchases.Entities;
using MarketZone.Domain.Purchases.Enums;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MarketZone.Application.Features.Purchases.Commands.CreatePurchaseInvoice
{
    public class CreatePurchaseInvoiceCommandHandler(IPurchaseInvoiceRepository purchaseInvoiceRepository, IUnitOfWork unitOfWork, IMapper mapper, IPurchaseInvoiceNumberGenerator numberGenerator) : IRequestHandler<CreatePurchaseInvoiceCommand, BaseResult<long>>
    {
        public async Task<BaseResult<long>> Handle(CreatePurchaseInvoiceCommand request, CancellationToken cancellationToken)
        {
            if (request.Details == null || !request.Details.Any())
            {
                return new Error(ErrorCode.FieldDataInvalid, "At least one line is required", nameof(request.Details));
            }
            var invoice = mapper.Map<PurchaseInvoice>(request);
            invoice.SetStatus(PurchaseInvoiceStatus.Draft);
            if (string.IsNullOrWhiteSpace(invoice.InvoiceNumber))
            {
                var nextNumber = await numberGenerator.GenerateAsync(cancellationToken);
                invoice.SetInvoiceNumber(nextNumber);
            }

            // Details are mapped via AutoMapper now

            await purchaseInvoiceRepository.AddAsync(invoice);
            await unitOfWork.SaveChangesAsync();

            return invoice.Id;
        }
    }
}



