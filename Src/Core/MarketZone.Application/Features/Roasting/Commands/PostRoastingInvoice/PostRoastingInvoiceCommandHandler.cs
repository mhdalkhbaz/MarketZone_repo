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
            private readonly IUnitOfWork _unitOfWork;

            public PostRoastingInvoiceCommandHandler(
                IRoastingInvoiceRepository repository,
                IUnroastedProdcutBalanceRepository unroastedRepository,
                IUnitOfWork unitOfWork)
            {
                _repository = repository;
                _unroastedRepository = unroastedRepository;
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
            // This method should add the roasted product to ProductBalance
            // For now, we'll use a simple approach - you might want to inject IProductBalanceRepository
            // or use the existing RoastingService logic
            
            // TODO: Implement proper inventory management for roasted products
            // This could involve:
            // 1. Adding to ProductBalance table
            // 2. Creating InventoryHistory records
            // 3. Updating product availability
            
            // For demonstration, we'll just log or handle this in a service
            // You can implement this based on your existing inventory management logic
        }
    }
}
