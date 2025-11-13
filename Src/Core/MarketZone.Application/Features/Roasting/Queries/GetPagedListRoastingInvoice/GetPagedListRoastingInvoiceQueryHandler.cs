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
using MarketZone.Domain.Cash.Enums;
using System.Linq;

namespace MarketZone.Application.Features.Roasting.Queries.GetPagedListRoastingInvoice
{
    public class GetPagedListRoastingInvoiceQueryHandler : IRequestHandler<GetPagedListRoastingInvoiceQuery, PagedResponse<RoastingInvoiceDto>>
    {
        private readonly IRoastingInvoiceRepository _repository;
        private readonly IMapper _mapper;
        private readonly IProductBalanceRepository _productBalanceRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public GetPagedListRoastingInvoiceQueryHandler(
            IRoastingInvoiceRepository repository, 
            IMapper mapper, 
            IProductBalanceRepository productBalanceRepository,
            IEmployeeRepository employeeRepository)
        {
            _repository = repository;
            _mapper = mapper;
            _productBalanceRepository = productBalanceRepository;
            _employeeRepository = employeeRepository;
        }

        public async Task<PagedResponse<RoastingInvoiceDto>> Handle(GetPagedListRoastingInvoiceQuery request, CancellationToken cancellationToken)
        {
            var pagedResult = await _repository.GetPagedListAsync(request.PageNumber, request.PageSize);
            var mappedData = _mapper.Map<List<RoastingInvoiceDto>>(pagedResult.Data);

            // Collect all raw product ids from details in this page
            var rawProductIds = mappedData
                .SelectMany(i => i.Details)
                .Select(d => d.RawProductId)
                .Distinct()
                .ToList();

            // Load balances and map to dictionary for quick lookup
            var balances = await _productBalanceRepository.GetAllProductBalanceAsync();
            var balanceByProductId = balances
                .Where(b => rawProductIds.Contains(b.ProductId))
                .ToDictionary(b => b.ProductId, b => b);

            // Collect all employee ids from invoices
            var employeeIds = mappedData
                .Where(i => i.EmployeeId.HasValue)
                .Select(i => i.EmployeeId.Value)
                .Distinct()
                .ToList();

            // Load employee currencies using repository method
            var employeesByEmployeeId = await _employeeRepository.GetEmployeeCurrenciesAsync(employeeIds, cancellationToken);

            // Fill PurchasePrice in each detail from ProductBalance and Currency from Employee
            foreach (var invoice in mappedData)
            {
                // Fill PurchasePrice for details
                foreach (var detail in invoice.Details)
                {
                    if (balanceByProductId.TryGetValue(detail.RawProductId, out var bal))
                        detail.PurchasePrice = bal.AverageCost;
                }

                // Fill Currency from employee
                if (invoice.EmployeeId.HasValue && employeesByEmployeeId.TryGetValue(invoice.EmployeeId.Value, out var currency))
                {
                    invoice.Currency = currency;
                }
            }

            return PagedResponse<RoastingInvoiceDto>.Ok(new PaginationResponseDto<RoastingInvoiceDto>(mappedData, pagedResult.Count, pagedResult.PageNumber, pagedResult.PageSize));
        }
    }
}
