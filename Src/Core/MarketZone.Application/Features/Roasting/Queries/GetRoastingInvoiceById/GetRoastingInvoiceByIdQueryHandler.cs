using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MarketZone.Application.Features.Roasting.Queries.GetRoastingInvoiceById;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Roasting.DTOs;
using System.Linq;

namespace MarketZone.Application.Features.Roasting.Queries.GetRoastingInvoiceById
{
    public class GetRoastingInvoiceByIdQueryHandler : IRequestHandler<GetRoastingInvoiceByIdQuery, BaseResult<RoastingInvoiceDto>>
    {
        private readonly IRoastingInvoiceRepository _repository;
        private readonly IMapper _mapper;
        private readonly IEmployeeRepository _employeeRepository;

        public GetRoastingInvoiceByIdQueryHandler(IRoastingInvoiceRepository repository, IMapper mapper, IEmployeeRepository employeeRepository)
        {
            _repository = repository;
            _mapper = mapper;
            _employeeRepository = employeeRepository;
        }

        public async Task<BaseResult<RoastingInvoiceDto>> Handle(GetRoastingInvoiceByIdQuery request, CancellationToken cancellationToken)
        {
            var roastingInvoice = await _repository.GetWithDetailsByIdAsync(request.Id);
            if (roastingInvoice == null)
            {
                throw new InvalidOperationException($"Roasting invoice with ID {request.Id} not found.");
            }

            var invoiceDto = _mapper.Map<RoastingInvoiceDto>(roastingInvoice);

            // Fill Currency from employee
            if (invoiceDto.EmployeeId.HasValue)
            {
                var employeeCurrencies = await _employeeRepository.GetEmployeeCurrenciesAsync(new[] { invoiceDto.EmployeeId.Value }.ToList(), cancellationToken);
                if (employeeCurrencies.TryGetValue(invoiceDto.EmployeeId.Value, out var currency))
                {
                    invoiceDto.Currency = currency;
                }
            }

            return new BaseResult<RoastingInvoiceDto> { Success = true, Data = invoiceDto };
        }
    }
}
