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
            private readonly IUnroastedProdcutBalanceRepository _unroastedRepository;
            private readonly IMapper _mapper;
            private readonly IUnitOfWork _unitOfWork;

            public CreateRoastingInvoiceCommandHandler(
                IRoastingInvoiceRepository repository,
                IRoastingInvoiceNumberGenerator numberGenerator,
                IUnroastedProdcutBalanceRepository unroastedRepository,
                IMapper mapper,
                IUnitOfWork unitOfWork)
            {
                _repository = repository;
                _numberGenerator = numberGenerator;
                _unroastedRepository = unroastedRepository;
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
                // Check and reserve available quantity
                var unroastedBalance = await _unroastedRepository.GetByProductIdAsync(detailItem.ProductId, cancellationToken);
                if (unroastedBalance == null)
                {
                    throw new InvalidOperationException($"Unroasted balance not found for product {detailItem.ProductId}");
                }

                if (unroastedBalance.AvailableQty < detailItem.QuantityKg)
                {
                    throw new InvalidOperationException($"Insufficient available quantity for product {detailItem.ProductId}. Available: {unroastedBalance.AvailableQty}, Requested: {detailItem.QuantityKg}");
                }

                // Reserve the quantity (reduce AvailableQty)
                unroastedBalance.Reserve(detailItem.QuantityKg);
                _unroastedRepository.Update(unroastedBalance);

                var detail = new RoastingInvoiceDetail(
                    roastingInvoice.Id,
                    detailItem.ProductId,
                    detailItem.QuantityKg,
                    detailItem.RoastPricePerKg,
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
