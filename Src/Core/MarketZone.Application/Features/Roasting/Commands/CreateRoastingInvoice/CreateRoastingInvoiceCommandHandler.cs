using AutoMapper;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Interfaces.Services;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Roasting.Entities;
using MarketZone.Domain.Inventory.Entities;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MarketZone.Application.Features.Roasting.Commands.CreateRoastingInvoice
{
            public class CreateRoastingInvoiceCommandHandler : IRequestHandler<CreateRoastingInvoiceCommand, BaseResult<long>>
        {
            private readonly IRoastingInvoiceRepository _repository;
            private readonly IRoastingInvoiceNumberGenerator _numberGenerator;
            private readonly IProductBalanceRepository _productBalanceRepository;
            private readonly IMapper _mapper;
            private readonly IUnitOfWork _unitOfWork;

            public CreateRoastingInvoiceCommandHandler(
                IRoastingInvoiceRepository repository,
                IRoastingInvoiceNumberGenerator numberGenerator,
                IProductBalanceRepository productBalanceRepository,
                IMapper mapper,
                IUnitOfWork unitOfWork)
            {
                _repository = repository;
                _numberGenerator = numberGenerator;
                _productBalanceRepository = productBalanceRepository;
                _mapper = mapper;
                _unitOfWork = unitOfWork;
            }

        public async Task<BaseResult<long>> Handle(CreateRoastingInvoiceCommand request, CancellationToken cancellationToken)
        {
            // Validate that details are not empty
            if (!request.Details.Any())
            {
                throw new InvalidOperationException("Roasting invoice must have at least one detail line.");
            }

            // Generate invoice number if not provided
            if (string.IsNullOrEmpty(request.InvoiceNumber))
            {
                request.InvoiceNumber = await _numberGenerator.GenerateAsync();
            }

            // Create the roasting invoice
            var roastingInvoice = _mapper.Map<RoastingInvoice>(request);

            // Add details manually since AutoMapper ignores them
            foreach (var detailItem in request.Details)
            {
                // Check and reserve available quantity only if RawProductId is provided
                if (detailItem.RawProductId.HasValue)
                {
                    var rawProductBalance = await _productBalanceRepository.GetByProductIdAsync(detailItem.RawProductId.Value, cancellationToken);
                    if (rawProductBalance == null)
                    {
                        throw new InvalidOperationException($"Raw product balance not found for product {detailItem.RawProductId}");
                    }

                    if (rawProductBalance.AvailableQty < detailItem.QuantityKg)
                    {
                        throw new InvalidOperationException($"Insufficient available quantity for product {detailItem.RawProductId}. Available: {rawProductBalance.AvailableQty}, Requested: {detailItem.QuantityKg}");
                    }

                    // Reserve the quantity (reduce AvailableQty)
                    rawProductBalance.Adjust(0, -detailItem.QuantityKg);
                    _productBalanceRepository.Update(rawProductBalance);
                }

                var detail = new RoastingInvoiceDetail(
                    roastingInvoice.Id,
                    detailItem.ReadyProductId,
                    detailItem.RawProductId,
                    detailItem.QuantityKg,
                    detailItem.RoastPricePerKg,
                    detailItem.CommissionPerKgOverride,
                    0, // ActualQuantityAfterRoasting = 0 عند الإنشاء
                    detailItem.Notes ?? string.Empty);

                roastingInvoice.AddDetail(detail);
            }

            await _repository.AddAsync(roastingInvoice);
            await _unitOfWork.SaveChangesAsync();

            return new BaseResult<long> { Success = true, Data = roastingInvoice.Id };
        }
    }
}
