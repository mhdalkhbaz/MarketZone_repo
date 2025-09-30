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
            private readonly IProductBalanceRepository _productBalanceRepository;
            private readonly IMapper _mapper;
            private readonly IUnitOfWork _unitOfWork;

            public UpdateRoastingInvoiceCommandHandler(
                IRoastingInvoiceRepository repository,
                IProductBalanceRepository productBalanceRepository,
                IMapper mapper,
                IUnitOfWork unitOfWork)
            {
                _repository = repository;
                _productBalanceRepository = productBalanceRepository;
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
                var rawProductBalance = await _productBalanceRepository.GetByProductIdAsync(detail.RawProductId, cancellationToken);
                if (rawProductBalance != null)
                {
                    rawProductBalance.Adjust(0, detail.QuantityKg); // Release the reserved quantity
                    _productBalanceRepository.Update(rawProductBalance);
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
                    RoastingInvoiceDetail existingDetail = null;
                    if (detailItem.Id.HasValue)
                    {
                        existingDetail = roastingInvoice.Details.FirstOrDefault(d => d.Id == detailItem.Id);
                    }

                    // Check and reserve available quantity for new detail state
                    var rawProductBalance = await _productBalanceRepository.GetByProductIdAsync(detailItem.RawProductId, cancellationToken);
                    if (rawProductBalance == null)
                    {
                        throw new InvalidOperationException($"Raw product balance not found for product {detailItem.RawProductId}");
                    }

                    if (rawProductBalance.AvailableQty < detailItem.QuantityKg)
                    {
                        throw new InvalidOperationException($"Insufficient available quantity for product {detailItem.RawProductId}. Available: {rawProductBalance.AvailableQty}, Requested: {detailItem.QuantityKg}");
                    }

                    rawProductBalance.Adjust(0, -detailItem.QuantityKg);
                    _productBalanceRepository.Update(rawProductBalance);

                    if (existingDetail != null)
                    {
                        existingDetail.Update(detailItem.RawProductId, detailItem.QuantityKg, detailItem.Notes, detailItem.RoastingCost);
                    }
                    else
                    {
                        var newDetail = new RoastingInvoiceDetail(
                            roastingInvoice.Id,
                            detailItem.RawProductId,
                            detailItem.QuantityKg,
                            detailItem.Notes,
                            detailItem.RoastingCost);

                        roastingInvoice.AddDetail(newDetail);
                    }
                }
            }

            _repository.Update(roastingInvoice);
            await _unitOfWork.SaveChangesAsync();

            return new BaseResult<long> { Success = true, Data = roastingInvoice.Id };
        }
    }
}
