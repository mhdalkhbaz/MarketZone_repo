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
    public class CreatePurchaseInvoiceCommandHandler(IPurchaseInvoiceRepository purchaseInvoiceRepository, IProductRepository productRepository, IUnitOfWork unitOfWork, IMapper mapper, IPurchaseInvoiceNumberGenerator numberGenerator) : IRequestHandler<CreatePurchaseInvoiceCommand, BaseResult<long>>
    {
        public async Task<BaseResult<long>> Handle(CreatePurchaseInvoiceCommand request, CancellationToken cancellationToken)
        {
            if (request.Details == null || !request.Details.Any())
            {
                return new Error(ErrorCode.FieldDataInvalid, "At least one line is required", nameof(request.Details));
            }

            // التحقق من أن المنتجات المختارة صالحة للشراء (جاهز أو ني فقط)
            var productIds = request.Details.Select(d => d.ProductId).Distinct().ToList();
            var products = await productRepository.GetByIdsAsync(productIds, cancellationToken);
            
            foreach (var detail in request.Details)
            {
                if (!products.TryGetValue(detail.ProductId, out var product))
                {
                    return new Error(ErrorCode.NotFound, $"Product with ID {detail.ProductId} not found", nameof(detail.ProductId));
                }

                // التحقق من أن المنتج ليس محمص (المنتجات المحمصة لا تدخل في فاتورة الشراء)
                if (product.RawProductId.HasValue)
                {
                    return new Error(ErrorCode.FieldDataInvalid, $"Roasted products cannot be purchased directly. Product: {product.Name}", nameof(detail.ProductId));
                }
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



