using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MarketZone.Application.Helpers;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.DTOs;

namespace MarketZone.Application.Features.Cash.CashRegisters.Queries.GetCashRegisterById
{
    public class GetCashRegisterByIdQueryHandler(ICashRegisterRepository cashRegisterRepository, ITranslator translator, IMapper mapper) : IRequestHandler<GetCashRegisterByIdQuery, BaseResult<CashRegisterDto>>
    {
        public async Task<BaseResult<CashRegisterDto>> Handle(GetCashRegisterByIdQuery request, CancellationToken cancellationToken)
        {
            var cashRegister = await cashRegisterRepository.GetByIdAsync(request.Id);

            if (cashRegister is null)
            {
                return new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.CashRegisterMessages.CashRegister_NotFound_with_id(request.Id)), nameof(request.Id));
            }

            return mapper.Map<CashRegisterDto>(cashRegister);
        }
    }
}
