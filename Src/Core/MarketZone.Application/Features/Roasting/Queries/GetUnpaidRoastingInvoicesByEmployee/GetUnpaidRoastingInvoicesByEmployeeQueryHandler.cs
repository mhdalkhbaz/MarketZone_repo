using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Helpers;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.Enums;
using MarketZone.Domain.Roasting.DTOs;
using MarketZone.Domain.Roasting.Enums;

namespace MarketZone.Application.Features.Roasting.Queries.GetUnpaidRoastingInvoicesByEmployee
{
    public class GetUnpaidRoastingInvoicesByEmployeeQueryHandler(IRoastingInvoiceRepository roastingInvoiceRepository, IPaymentRepository paymentRepository, ITranslator translator) : IRequestHandler<GetUnpaidRoastingInvoicesByEmployeeQuery, BaseResult<List<RoastingInvoiceUnpaidDto>>>
    {
        public async Task<BaseResult<List<RoastingInvoiceUnpaidDto>>> Handle(GetUnpaidRoastingInvoicesByEmployeeQuery request, CancellationToken cancellationToken)
        {
            // التحقق من وجود الموظف
            var employeeExists = await roastingInvoiceRepository.EmployeeExistsAsync(request.EmployeeId);
            if (!employeeExists)
            {
                return new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.EmployeeMessages.Employee_NotFound_with_id(request.EmployeeId)), nameof(request.EmployeeId));
            }

            // الحصول على فواتير التحميص غير المسددة للموظف
            var unpaidInvoices = await roastingInvoiceRepository.GetUnpaidInvoicesByEmployeeAsync(request.EmployeeId, cancellationToken);

            return BaseResult<List<RoastingInvoiceUnpaidDto>>.Ok(unpaidInvoices);
        }
    }
}
