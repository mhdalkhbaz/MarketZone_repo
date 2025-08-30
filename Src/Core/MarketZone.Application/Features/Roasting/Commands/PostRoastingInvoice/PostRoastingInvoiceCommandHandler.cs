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
            private readonly IUnroastedProdcutBalanceRepository _unroastedRepository;
            private readonly IProductBalanceRepository _productBalanceRepository;
            private readonly IInventoryHistoryRepository _inventoryHistoryRepository;
            private readonly IUnitOfWork _unitOfWork;

            public PostRoastingInvoiceCommandHandler(
                IRoastingInvoiceRepository repository,
                IUnroastedProdcutBalanceRepository unroastedRepository,
                IProductBalanceRepository productBalanceRepository,
                IInventoryHistoryRepository inventoryHistoryRepository,
                IUnitOfWork unitOfWork)
            {
                _repository = repository;
                _unroastedRepository = unroastedRepository;
                _productBalanceRepository = productBalanceRepository;
                _inventoryHistoryRepository = inventoryHistoryRepository;
                _unitOfWork = unitOfWork;
            }

        public async Task<BaseResult<long>> Handle(PostRoastingInvoiceCommand request, CancellationToken cancellationToken)
        {
            var roastingInvoice = await _repository.GetWithDetailsByIdAsync(request.Id);
            if (roastingInvoice == null)
            {
                throw new InvalidOperationException($"Roasting invoice with ID {request.Id} not found.");
            }

            if (roastingInvoice.Status == RoastingInvoiceStatus.Posted)
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

                var unroastedBalance = await _unroastedRepository.GetByProductIdAsync(detail.ProductId, cancellationToken);
                if (unroastedBalance == null)
                {
                    throw new InvalidOperationException($"Unroasted balance not found for product {detail.ProductId}");
                }

                // Check if we have enough quantity (Qty should be >= requested quantity)
                if (unroastedBalance.Qty < detail.QuantityKg)
                {
                    throw new InvalidOperationException($"Insufficient quantity for product {detail.ProductId}. Available: {unroastedBalance.Qty}, Requested: {detail.QuantityKg}");
                }

                // Consume the unroasted quantity (reduce Qty only)
                unroastedBalance.Decrease(detail.QuantityKg);
                _unroastedRepository.Update(unroastedBalance);

                // Update the detail with actual quantity
                UpdateDetailWithActualQuantity(detail, actualQuantity);

                // Add the actual roasted quantity to ProductBalance
                await AddRoastedProductToInventory(detail.ProductId, actualQuantity, cancellationToken);
            }

            // Set status to Posted
            roastingInvoice.SetStatus(RoastingInvoiceStatus.Posted);
            _repository.Update(roastingInvoice);

            await _unitOfWork.SaveChangesAsync();

            return new BaseResult<long> { Success = true, Data = roastingInvoice.Id };
        }

        private void UpdateDetailWithActualQuantity(RoastingInvoiceDetail detail, decimal actualQuantity)
        {
            // Update the actual quantity using reflection since the property is private set
            var property = typeof(RoastingInvoiceDetail).GetProperty("ActualQuantityAfterRoasting");
            property?.SetValue(detail, actualQuantity);
        }

        private async Task AddRoastedProductToInventory(long productId, decimal actualQuantity, CancellationToken cancellationToken)
        {
            // Get or create ProductBalance for the roasted product
            var balance = await _productBalanceRepository.GetByProductIdAsync(productId, cancellationToken);

            if (balance == null)
            {
                // Create new ProductBalance for roasted product
                balance = new ProductBalance(productId, actualQuantity, actualQuantity);
                await _productBalanceRepository.AddAsync(balance);
            }
            else
            {
                // Adjust existing ProductBalance
                balance.Adjust(actualQuantity, actualQuantity);
                _productBalanceRepository.Update(balance);
            }

            // Create InventoryHistory record for tracking
            var inventoryHistory = new InventoryHistory(
                productId, 
                "Roast", 
                null, 
                actualQuantity, 
                DateTime.UtcNow, 
                $"Roasted product added to inventory - Actual quantity: {actualQuantity}kg");

            await _inventoryHistoryRepository.AddAsync(inventoryHistory);
        }
    }
}
