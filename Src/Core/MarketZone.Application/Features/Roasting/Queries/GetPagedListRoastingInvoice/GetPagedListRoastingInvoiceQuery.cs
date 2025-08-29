using MarketZone.Application.DTOs;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Roasting.DTOs;

namespace MarketZone.Application.Features.Roasting.Queries.GetPagedListRoastingInvoice
{
    public class GetPagedListRoastingInvoiceQuery : IRequest<PagedResponse<RoastingInvoiceDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
