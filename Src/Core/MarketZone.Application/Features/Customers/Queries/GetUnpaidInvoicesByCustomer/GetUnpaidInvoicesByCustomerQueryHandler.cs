using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Helpers;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.Enums;
using MarketZone.Domain.Sales.DTOs;
using MarketZone.Domain.Sales.Enums;

namespace MarketZone.Application.Features.Customers.Queries.GetUnpaidInvoicesByCustomer
{
    public class GetUnpaidInvoicesByCustomerQueryHandler(ISalesInvoiceRepository salesInvoiceRepository, IPaymentRepository paymentRepository, ITranslator translator) : IRequestHandler<GetUnpaidInvoicesByCustomerQuery, BaseResult<List<SalesInvoiceDto>>>
    {
        public async Task<BaseResult<List<SalesInvoiceDto>>> Handle(GetUnpaidInvoicesByCustomerQuery request, CancellationToken cancellationToken)
        {
            // التحقق من وجود الزبون
            var customerExists = await salesInvoiceRepository.CustomerExistsAsync(request.CustomerId);
            if (!customerExists)
            {
                return new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.CustomerMessages.Customer_NotFound_with_id(request.CustomerId)), nameof(request.CustomerId));
            }

            // الحصول على الفواتير غير المسددة للزبون
            var unpaidInvoices = await salesInvoiceRepository.GetUnpaidInvoicesByCustomerAsync(request.CustomerId, cancellationToken);

            return BaseResult<List<SalesInvoiceDto>>.Ok(unpaidInvoices);
        }
    }
}
