using MarketZone.Application.Interfaces;
using MarketZone.Application.Parameters;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Roasting.DTOs;

namespace MarketZone.Application.Features.Roasting.Queries.GetPagedListRoastingInvoice
{
    public class GetPagedListRoastingInvoiceQuery : RoastingInvoiceFilter, IRequest<PagedResponse<RoastingInvoiceDto>>
    {
    }
}
