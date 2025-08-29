using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Roasting.DTOs;

namespace MarketZone.Application.Features.Roasting.Queries.GetRoastingInvoiceById
{
    public class GetRoastingInvoiceByIdQuery : IRequest<BaseResult<RoastingInvoiceDto>>
    {
        public long Id { get; set; }
    }
}
