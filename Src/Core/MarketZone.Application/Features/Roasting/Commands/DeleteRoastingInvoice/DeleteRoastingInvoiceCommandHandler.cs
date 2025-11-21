using System;
using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Roasting.Enums;
using MarketZone.Domain.Inventory.Entities;
using MarketZone.Application.DTOs;

namespace MarketZone.Application.Features.Roasting.Commands.DeleteRoastingInvoice
{
    public class DeleteRoastingInvoiceCommandHandler : IRequestHandler<DeleteRoastingInvoiceCommand, BaseResult>
    {
	private readonly IRoastingInvoiceRepository _repository;
	private readonly IProductBalanceRepository _productBalanceRepository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly ITranslator _translator;

	public DeleteRoastingInvoiceCommandHandler(
		IRoastingInvoiceRepository repository,
		IProductBalanceRepository productBalanceRepository,
		IUnitOfWork unitOfWork,
		ITranslator translator)
	{
		_repository = repository;
		_productBalanceRepository = productBalanceRepository;
		_unitOfWork = unitOfWork;
		_translator = translator;
	}

        public async Task<BaseResult> Handle(DeleteRoastingInvoiceCommand request, CancellationToken cancellationToken)
        {
		var roastingInvoice = await _repository.GetWithDetailsByIdAsync(request.Id);
		if (roastingInvoice == null)
		{
			var message = _translator.GetString(new TranslatorMessageDto("RoastingInvoice_NotFound_With_ID", new[] { request.Id.ToString() }));
			return new Error(ErrorCode.NotFound, message, nameof(request.Id));
		}

		// Prevent deletion if invoice is posted
		if (roastingInvoice.Status == RoastingInvoiceStatus.Posted)
		{
			return new Error(ErrorCode.AccessDenied, _translator.GetString("Cannot_Delete_Posted_Roasting_Invoice"), nameof(request.Id));
		}

            // Release all reserved quantities (إرجاع Qty و AvailableQty)
            foreach (var detail in roastingInvoice.Details)
            {
                var rawProductBalance = await _productBalanceRepository.GetByProductIdAsync(detail.RawProductId, cancellationToken);
                if (rawProductBalance != null)
                {
                    // إرجاع Qty و AvailableQty التي تم تقليلها عند الإنشاء
                    rawProductBalance.Adjust(detail.QuantityKg, detail.QuantityKg);
                    _productBalanceRepository.Update(rawProductBalance);
                }
            }

            _repository.Delete(roastingInvoice);
            await _unitOfWork.SaveChangesAsync();

            return new BaseResult { Success = true };
        }
    }
}
