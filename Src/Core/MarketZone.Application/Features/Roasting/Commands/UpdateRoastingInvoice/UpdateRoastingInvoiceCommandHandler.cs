using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MarketZone.Application.Features.Roasting.Commands.UpdateRoastingInvoice;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Roasting.Entities;
using MarketZone.Domain.Roasting.Enums;
using MarketZone.Domain.Inventory.Entities;

namespace MarketZone.Application.Features.Roasting.Commands.UpdateRoastingInvoice
{
            public class UpdateRoastingInvoiceCommandHandler : IRequestHandler<UpdateRoastingInvoiceCommand, BaseResult<long>>
        {
            private readonly IRoastingInvoiceRepository _repository;
            private readonly IUnroastedProdcutBalanceRepository _unroastedRepository;
            private readonly IMapper _mapper;
            private readonly IUnitOfWork _unitOfWork;

            public UpdateRoastingInvoiceCommandHandler(
                IRoastingInvoiceRepository repository,
                IUnroastedProdcutBalanceRepository unroastedRepository,
                IMapper mapper,
                IUnitOfWork unitOfWork)
            {
                _repository = repository;
                _unroastedRepository = unroastedRepository;
                _mapper = mapper;
                _unitOfWork = unitOfWork;
            }

        public async Task<BaseResult<long>> Handle(UpdateRoastingInvoiceCommand request, CancellationToken cancellationToken)
        {
            var roastingInvoice = await _repository.GetWithDetailsByIdAsync(request.Id);
            if (roastingInvoice == null)
            {
                throw new InvalidOperationException($"Roasting invoice with ID {request.Id} not found.");
            }

            // Prevent updates if invoice is posted
            if (roastingInvoice.Status == RoastingInvoiceStatus.Posted)
            {
                throw new InvalidOperationException("Cannot update a posted roasting invoice.");
            }

            // Release all previously reserved quantities
            foreach (var detail in roastingInvoice.Details)
            {
                var unroastedBalance = await _unroastedRepository.GetByProductIdAsync(detail.RawProductId.Value, cancellationToken);
                if (unroastedBalance != null)
                {
                    unroastedBalance.Release(detail.QuantityKg);
                    _unroastedRepository.Update(unroastedBalance);
                }
            }

            // Update basic properties
            roastingInvoice.SetInvoiceNumber(request.InvoiceNumber);
            // Note: We can't update InvoiceDate and TotalAmount directly as they're private set
            // This would need to be handled differently if needed

            // Handle details
            foreach (var detailItem in request.Details)
            {
                if (detailItem.IsDeleted && detailItem.Id.HasValue)
                {
                    // Remove existing detail
                    var existingDetail = roastingInvoice.Details.FirstOrDefault(d => d.Id == detailItem.Id);
                    if (existingDetail != null)
                    {
                        roastingInvoice.RemoveDetail(existingDetail);
                    }
                }
                else if (!detailItem.IsDeleted)
                {
                    if (detailItem.Id.HasValue)
                    {
                        // Update existing detail
                        var existingDetail = roastingInvoice.Details.FirstOrDefault(d => d.Id == detailItem.Id);
                        if (existingDetail != null)
                        {
                            // Update properties (this would need setters in the entity)
                            // For now, we'll remove and add new
                            roastingInvoice.RemoveDetail(existingDetail);
                        }
                    }

                    // Check and reserve available quantity for new detail
                    var unroastedBalance = await _unroastedRepository.GetByProductIdAsync(detailItem.RawProductId.Value, cancellationToken);
                    if (unroastedBalance == null)
                    {
                        throw new InvalidOperationException($"Unroasted balance not found for product {detailItem.RawProductId}");
                    }

                    if (unroastedBalance.AvailableQty < detailItem.QuantityKg)
                    {
                        throw new InvalidOperationException($"Insufficient available quantity for product {detailItem.RawProductId}. Available: {unroastedBalance.AvailableQty}, Requested: {detailItem.QuantityKg}");
                    }

                    // Reserve the quantity (reduce AvailableQty)
                    unroastedBalance.Reserve(detailItem.QuantityKg);
                    _unroastedRepository.Update(unroastedBalance);

                    // Add new detail
                    var newDetail = new RoastingInvoiceDetail(
                        roastingInvoice.Id,
                        detailItem.ReadyProductId,
                        detailItem.RawProductId,
                        detailItem.QuantityKg,
                        detailItem.RoastPricePerKg,
                        detailItem.CommissionPerKgOverride,
                        0, // ActualQuantityAfterRoasting = 0 عند الإنشاء/التحديث
                        detailItem.Notes ?? string.Empty);
                    
                    roastingInvoice.AddDetail(newDetail);
                }
            }

            _repository.Update(roastingInvoice);
            await _unitOfWork.SaveChangesAsync();

            return new BaseResult<long> { Success = true, Data = roastingInvoice.Id };
        }
    }
}
