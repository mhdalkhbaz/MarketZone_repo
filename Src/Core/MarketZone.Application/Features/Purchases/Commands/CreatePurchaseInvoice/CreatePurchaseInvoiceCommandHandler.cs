using AutoMapper;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Interfaces.Services;
using MarketZone.Domain.Cash.Enums;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Purchases.Entities;
using MarketZone.Domain.Purchases.Enums;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.DTOs;

namespace MarketZone.Application.Features.Purchases.Commands.CreatePurchaseInvoice
{
    public class CreatePurchaseInvoiceCommandHandler(IPurchaseInvoiceRepository purchaseInvoiceRepository, IProductRepository productRepository, ISupplierRepository supplierRepository, IUnitOfWork unitOfWork, IMapper mapper, IInvoiceNumberGenerator numberGenerator, ITranslator translator) : IRequestHandler<CreatePurchaseInvoiceCommand, BaseResult<long>>
    {
        public async Task<BaseResult<long>> Handle(CreatePurchaseInvoiceCommand request, CancellationToken cancellationToken)
        {
            var productIds = request.Details.Select(d => d.ProductId).Distinct().ToList();
            var products = await productRepository.GetByIdsAsync(productIds, cancellationToken);
            
            foreach (var detail in request.Details)
            {
                if (!products.TryGetValue(detail.ProductId, out var product))
                {
                    var message = translator.GetString(new TranslatorMessageDto("Product_With_ID_Not_Found", new[] { detail.ProductId.ToString() }));
                    return new Error(ErrorCode.NotFound, message, nameof(detail.ProductId));
                }
                if (product.RawProductId.HasValue)
                {
                    var message = translator.GetString(new TranslatorMessageDto("Roasted_Products_Cannot_Be_Purchased_Directly", new[] { product.Name }));
                    return new Error(ErrorCode.FieldDataInvalid, message, nameof(detail.ProductId));
                }
            }

            // Default invoice currency from supplier if not provided
            if (!request.Currency.HasValue)
            {
                var supplier = await supplierRepository.GetByIdAsync(request.SupplierId);
                if (supplier?.Currency != null)
                {
                    request.Currency = supplier.Currency;
                }
            }

            var invoice = mapper.Map<PurchaseInvoice>(request);
            invoice.SetStatus(PurchaseInvoiceStatus.Draft);
            if (string.IsNullOrWhiteSpace(invoice.InvoiceNumber))
            {
                var nextNumber = await numberGenerator.GenerateAsync(MarketZone.Domain.Cash.Enums.InvoiceType.PurchaseInvoice, cancellationToken);
                invoice.SetInvoiceNumber(nextNumber);
            }

            await purchaseInvoiceRepository.AddAsync(invoice);
            await unitOfWork.SaveChangesAsync();

            return invoice.Id;
        }
    }
}



