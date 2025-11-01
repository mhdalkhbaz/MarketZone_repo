using System.Collections.Generic;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Roasting.DTOs;

namespace MarketZone.Application.Features.Roasting.Queries.GetUnpaidRoastingInvoicesByEmployee
{
    public class GetUnpaidRoastingInvoicesByEmployeeQuery : IRequest<BaseResult<List<RoastingInvoiceUnpaidDto>>>
    {
        public long EmployeeId { get; set; }
    }
}
