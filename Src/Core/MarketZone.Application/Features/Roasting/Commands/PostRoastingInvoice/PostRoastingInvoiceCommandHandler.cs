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
            private readonly IEmployeeRepository _employeeRepository;

            public PostRoastingInvoiceCommandHandler(
                IRoastingInvoiceRepository repository,
                // تم حذف unroastedRepository
                IProductBalanceRepository productBalanceRepository,
                IInventoryHistoryRepository inventoryHistoryRepository,
                IUnitOfWork unitOfWork,
                IProductRepository productRepository,
                IEmployeeRepository employeeRepository)
            {
                _repository = repository;
                // تم حذف _unroastedRepository
                _productBalanceRepository = productBalanceRepository;
                _inventoryHistoryRepository = inventoryHistoryRepository;
                _unitOfWork = unitOfWork;
                _productRepository = productRepository;
                _employeeRepository = employeeRepository;
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

            // Process each detail with its ready products
            foreach (var detailItem in request.Details)
            {
                var detail = roastingInvoice.Details.FirstOrDefault(d => d.Id == detailItem.DetailId);
                if (detail == null)
                {
                    throw new InvalidOperationException($"Detail with ID {detailItem.DetailId} not found.");
                }

                // Validate and process each ready product
                foreach (var readyDetail in detailItem.ReadyDetails)
                {
                    if (readyDetail.ActualQuantityAfterRoasting <= 0)
                    {
                        throw new InvalidOperationException($"Actual quantity must be greater than 0 for ready product {readyDetail.ReadyProductId}");
                    }

                    // Get raw product balance
                    var rawProductBalance = await _productBalanceRepository.GetByProductIdAsync(readyDetail.RawProductId, cancellationToken);
                    if (rawProductBalance == null)
                    {
                        throw new InvalidOperationException($"Raw product balance not found for product {readyDetail.RawProductId}");
                    }

                    // Check if we have enough quantity
                    //if (rawProductBalance.AvailableQty < readyDetail.ActualQuantityAfterRoasting)
                    //{
                    //    throw new InvalidOperationException($"Insufficient quantity for product {readyDetail.RawProductId}. Available: {rawProductBalance.AvailableQty}, Requested: {readyDetail.ActualQuantityAfterRoasting}");
                    //}

                    // Consume raw product quantity
                    rawProductBalance.Adjust(-detail.QuantityKg,0);
                    _productBalanceRepository.Update(rawProductBalance);

                    // Calculate values
                    var rawAvgCost = rawProductBalance.Qty > 0 ? (rawProductBalance.TotalValue / rawProductBalance.Qty) : 0;
                    var rawConsumedValue = rawAvgCost * readyDetail.ActualQuantityAfterRoasting;
                    var commissionValue = readyDetail.CommissionPerKg * readyDetail.ActualQuantityAfterRoasting;
                    var totalValue = rawConsumedValue + commissionValue;

                    // Add ready product to inventory and set its balance average cost to SalePricePerKg
                    await AddRoastedProductToInventoryWithValue(readyDetail.ReadyProductId, readyDetail.ActualQuantityAfterRoasting, totalValue, readyDetail.SalePricePerKg, cancellationToken);

                    // Create receipt record
                    var receipt = new RoastingInvoiceDetailReceipt(
                        roastingInvoice.Id,
                        detail.Id,
                        readyDetail.ReadyProductId,
                        readyDetail.ActualQuantityAfterRoasting,
                        readyDetail.SalePricePerKg,
                        readyDetail.RoastingCostPerKg,
                        readyDetail.CommissionPerKg,
                        readyDetail.NetSalePricePerKg
                    );

                    roastingInvoice.AddReceipt(receipt);
                    detail.AddReceipt(receipt);
                }
            }

            // Set status to Posted
            roastingInvoice.SetStatus(RoastingInvoiceStatus.Posted);
            _repository.Update(roastingInvoice);

            // Update employee balance if employee is assigned
            if (roastingInvoice.EmployeeId.HasValue)
            {
                await UpdateEmployeeBalance(roastingInvoice.EmployeeId.Value, roastingInvoice, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync();

            return new BaseResult<long> { Success = true, Data = roastingInvoice.Id };
        }

        private async Task AddRoastedProductToInventoryWithValue(long productId, decimal actualQuantity, decimal valueToAdd, decimal salePricePerKg, CancellationToken cancellationToken)
        {
            // Get or create ProductBalance for the roasted product
            var balance = await _productBalanceRepository.GetByProductIdAsync(productId, cancellationToken);

            if (balance == null)
            {
                // Create new ProductBalance for roasted product
                balance = new ProductBalance(productId, actualQuantity, actualQuantity, valueToAdd);
                // Set average cost from sale price per kg if provided
                if (salePricePerKg > 0)
                {
                    balance.SetAverageCost(salePricePerKg);
                }
                await _productBalanceRepository.AddAsync(balance);
            }
            else
            {
                // Adjust existing ProductBalance
                balance.AdjustWithValue(actualQuantity, actualQuantity, valueToAdd);
                // Override average cost from sale price per kg if provided
                if (salePricePerKg > 0)
                {
                    balance.SetAverageCost(salePricePerKg);
                }
                _productBalanceRepository.Update(balance);
            }

            // Create InventoryHistory record for tracking
            var inventoryHistory = new InventoryHistory(
                productId, 
                "Roast", 
                null, 
                actualQuantity, 
                DateTime.UtcNow, 
                $"Roasted product added to inventory - Actual quantity: {actualQuantity}kg, Added value: {valueToAdd}");

            await _inventoryHistoryRepository.AddAsync(inventoryHistory);
        }

        private async Task UpdateEmployeeBalance(long employeeId, RoastingInvoice roastingInvoice, CancellationToken cancellationToken)
        {
            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee == null)
            {
                throw new InvalidOperationException($"Employee with ID {employeeId} not found.");
            }

            // Calculate total roasting cost from all receipts
            decimal totalRoastingCost = 0;
            foreach (var receipt in roastingInvoice.Receipts)
            {
                totalRoastingCost += receipt.TotalRoastingCost;
            }

            // Update employee's Syrian money balance
            var currentSyrianMoney = employee.SyrianMoney ?? 0;
            employee.Update(
                employee.FirstName,
                employee.LastName,
                employee.Phone,
                employee.WhatsAppPhone,
                employee.Email,
                employee.Address,
                employee.JobTitle,
                employee.Salary,
                employee.HireDate,
                employee.IsActive,
                currentSyrianMoney + totalRoastingCost,
                employee.DollarMoney
            );

            _employeeRepository.Update(employee);
        }
    }
}
