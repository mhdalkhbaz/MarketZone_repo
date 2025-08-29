using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Roasting.Commands.DeleteRoastingInvoice
{
    public class DeleteRoastingInvoiceCommand : IRequest<BaseResult>
    {
        public long Id { get; set; }
    }
}
