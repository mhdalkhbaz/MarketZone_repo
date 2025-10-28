using System.Collections.Generic;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Sales.DTOs;

namespace MarketZone.Application.Features.Customers.Queries.GetUnpaidInvoicesByCustomer
{
    public class GetUnpaidInvoicesByCustomerQuery : IRequest<BaseResult<List<SalesInvoiceDto>>>
    {
        public long CustomerId { get; set; }
    }
}
