using System.Collections.Generic;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Purchases.DTOs;

namespace MarketZone.Application.Features.Suppliers.Queries.GetUnpaidInvoicesBySupplier
{
    public class GetUnpaidInvoicesBySupplierQuery : IRequest<BaseResult<List<PurchaseInvoiceUnpaidDto>>>
    {
        public long SupplierId { get; set; }
    }
}
