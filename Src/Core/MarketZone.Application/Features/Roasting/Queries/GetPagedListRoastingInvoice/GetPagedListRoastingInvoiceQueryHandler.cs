using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MarketZone.Application.DTOs;
using MarketZone.Application.Features.Roasting.Queries.GetPagedListRoastingInvoice;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Roasting.DTOs;

namespace MarketZone.Application.Features.Roasting.Queries.GetPagedListRoastingInvoice
{
    public class GetPagedListRoastingInvoiceQueryHandler : IRequestHandler<GetPagedListRoastingInvoiceQuery, PagedResponse<RoastingInvoiceDto>>
    {
        private readonly IRoastingInvoiceRepository _repository;
        private readonly IMapper _mapper;

        public GetPagedListRoastingInvoiceQueryHandler(IRoastingInvoiceRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PagedResponse<RoastingInvoiceDto>> Handle(GetPagedListRoastingInvoiceQuery request, CancellationToken cancellationToken)
        {
            var pagedResult = await _repository.GetPagedListAsync(request.PageNumber, request.PageSize);
            var mappedData = _mapper.Map<List<RoastingInvoiceDto>>(pagedResult.Data);

            return PagedResponse<RoastingInvoiceDto>.Ok(new PaginationResponseDto<RoastingInvoiceDto>(mappedData, pagedResult.Count, pagedResult.PageNumber, pagedResult.PageSize));
        }
    }
}
