using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MarketZone.Application.Features.Roasting.Queries.GetRoastingInvoiceById;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Roasting.DTOs;

namespace MarketZone.Application.Features.Roasting.Queries.GetRoastingInvoiceById
{
    public class GetRoastingInvoiceByIdQueryHandler : IRequestHandler<GetRoastingInvoiceByIdQuery, BaseResult<RoastingInvoiceDto>>
    {
        private readonly IRoastingInvoiceRepository _repository;
        private readonly IMapper _mapper;

        public GetRoastingInvoiceByIdQueryHandler(IRoastingInvoiceRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<BaseResult<RoastingInvoiceDto>> Handle(GetRoastingInvoiceByIdQuery request, CancellationToken cancellationToken)
        {
            var roastingInvoice = await _repository.GetWithDetailsByIdAsync(request.Id);
            if (roastingInvoice == null)
            {
                throw new InvalidOperationException($"Roasting invoice with ID {request.Id} not found.");
            }

            return new BaseResult<RoastingInvoiceDto> { Success = true, Data = _mapper.Map<RoastingInvoiceDto>(roastingInvoice) };
        }
    }
}
