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
            if (string.IsNullOrEmpty(request.InvoiceNumber))
                request.InvoiceNumber = await _numberGenerator.GenerateAsync();

            var roastingInvoice = _mapper.Map<RoastingInvoice>(request);

            foreach (var detailItem in request.Details)
            {
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
                    0,  
                    detailItem.Notes ?? string.Empty);

                roastingInvoice.AddDetail(detail);
            }

            await _repository.AddAsync(roastingInvoice);
            await _unitOfWork.SaveChangesAsync();

            return new BaseResult<long> { Success = true, Data = roastingInvoice.Id };
        }
    }
}
