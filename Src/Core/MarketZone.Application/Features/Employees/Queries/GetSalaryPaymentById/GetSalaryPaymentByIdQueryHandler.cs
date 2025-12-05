using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.DTOs;
using MarketZone.Application.Helpers;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Employees.DTOs;

namespace MarketZone.Application.Features.Employees.Queries.GetSalaryPaymentById
{
    public class GetSalaryPaymentByIdQueryHandler(
        ISalaryPaymentRepository repository,
        ITranslator translator) : IRequestHandler<GetSalaryPaymentByIdQuery, BaseResult<SalaryPaymentDto>>
    {
        public async Task<BaseResult<SalaryPaymentDto>> Handle(GetSalaryPaymentByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await repository.GetByIdWithIncludesAsync(request.Id, cancellationToken);
            if (entity == null)
            {
                var message = translator.GetString(TranslatorMessages.SalaryPaymentMessages.SalaryPayment_NotFound_with_id(request.Id));
                return new Error(ErrorCode.NotFound, message, nameof(request.Id));
            }

            var dto = new SalaryPaymentDto
            {
                Id = entity.Id,
                EmployeeId = entity.EmployeeId,
                EmployeeName = entity.Employee is null ? null : $"{entity.Employee.FirstName} {entity.Employee.LastName}".Trim(),
                Year = entity.Year,
                Month = entity.Month,
                Amount = entity.Amount,
                PaymentDate = entity.PaymentDate,
                CashRegisterId = entity.CashRegisterId,
                CashRegisterName = entity.CashRegister?.Name,
                Notes = entity.Notes,
                SalaryType = entity.SalaryType,
                DistributionTripId = entity.DistributionTripId,
                CreatedDateTime = entity.Created,
                LastModifiedDateTime = entity.LastModified
            };

            return BaseResult<SalaryPaymentDto>.Ok(dto);
        }
    }
}

