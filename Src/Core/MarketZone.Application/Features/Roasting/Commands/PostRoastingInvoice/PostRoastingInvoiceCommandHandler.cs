using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Roasting.Enums;
using MarketZone.Domain.Roasting.Entities;
using MarketZone.Domain.Inventory.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Reflection;

namespace MarketZone.Application.Features.Roasting.Commands.PostRoastingInvoice
{
            public class PostRoastingInvoiceCommandHandler : IRequestHandler<PostRoastingInvoiceCommand, BaseResult<long>>
        {
            private readonly IRoastingInvoiceRepository _repository;
            // تم حذف UnroastedProdcutBalanceRepository - نستخدم ProductBalanceRepository فقط
            private readonly IProductBalanceRepository _productBalanceRepository;
            private readonly IInventoryHistoryRepository _inventoryHistoryRepository;
            private readonly IUnitOfWork _unitOfWork;
            private readonly IProductRepository _productRepository;

            public PostRoastingInvoiceCommandHandler(
                IRoastingInvoiceRepository repository,
                // تم حذف unroastedRepository
                IProductBalanceRepository productBalanceRepository,
                IInventoryHistoryRepository inventoryHistoryRepository,
                IUnitOfWork unitOfWork,
                IProductRepository productRepository)
            {
                _repository = repository;
                // تم حذف _unroastedRepository
                _productBalanceRepository = productBalanceRepository;
                _inventoryHistoryRepository = inventoryHistoryRepository;
                _unitOfWork = unitOfWork;
                _productRepository = productRepository;
            }

        public async Task<BaseResult<long>> Handle(PostRoastingInvoiceCommand request, CancellationToken cancellationToken)
        {
            var roastingInvoice = await _repository.GetWithDetailsByIdAsync(request.Id);
            if (roastingInvoice == null)
            {
                throw new InvalidOperationException($"Roasting invoice with ID {request.Id} not found.");
            }

            if (roastingInvoice.Status == RoastingInvoiceStatus.Posted || roastingInvoice.Status == RoastingInvoiceStatus.Received)
            {
                throw new InvalidOperationException("Roasting invoice is already posted.");
            }

            // Validate that all details have actual quantities
            if (request.Details.Count != roastingInvoice.Details.Count)
            {
                throw new InvalidOperationException("All roasting invoice details must have actual quantities specified.");
            }

            // Create a dictionary for quick lookup
            var actualQuantities = request.Details.ToDictionary(d => d.DetailId, d => d.ActualQuantityAfterRoasting);

            // Consume the reserved quantities and add roasted product to inventory
            foreach (var detail in roastingInvoice.Details)
            {
                // Get the actual quantity for this detail
                if (!actualQuantities.TryGetValue(detail.Id, out var actualQuantity))
                {
                    throw new InvalidOperationException($"Actual quantity not provided for detail ID {detail.Id}");
                }

                if (actualQuantity <= 0)
                {
                    throw new InvalidOperationException($"Actual quantity must be greater than 0 for detail ID {detail.Id}");
                }

                var rawProductBalance = await _productBalanceRepository.GetByProductIdAsync(detail.RawProductId, cancellationToken);
                if (rawProductBalance == null)
                {
                    throw new InvalidOperationException($"Raw product balance not found for product {detail.RawProductId}");
                }

                // Check if we have enough quantity (AvailableQty should be >= requested quantity)
                if (rawProductBalance.AvailableQty < detail.QuantityKg)
                {
                    throw new InvalidOperationException($"Insufficient quantity for product {detail.RawProductId}. Available: {rawProductBalance.AvailableQty}, Requested: {detail.QuantityKg}");
                }
            }

            // Set status to Posted
            roastingInvoice.SetStatus(RoastingInvoiceStatus.Receiving);
            _repository.Update(roastingInvoice);

            await _unitOfWork.SaveChangesAsync();

            return new BaseResult<long> { Success = true, Data = roastingInvoice.Id };
        }
    }
}
