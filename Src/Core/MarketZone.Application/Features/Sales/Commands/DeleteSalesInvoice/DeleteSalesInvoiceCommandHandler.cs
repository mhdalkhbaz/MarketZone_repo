using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Helpers;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Sales.Enums;

namespace MarketZone.Application.Features.Sales.Commands.DeleteSalesInvoice
{
	public class DeleteSalesInvoiceCommandHandler : IRequestHandler<DeleteSalesInvoiceCommand, BaseResult>
	{
		private readonly ISalesInvoiceRepository _repository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly ITranslator _translator;
		private readonly IProductBalanceRepository _productBalanceRepository;

		public DeleteSalesInvoiceCommandHandler(
			ISalesInvoiceRepository repository, 
			IUnitOfWork unitOfWork, 
			ITranslator translator,
			IProductBalanceRepository productBalanceRepository)
		{
			_repository = repository;
			_unitOfWork = unitOfWork;
			_translator = translator;
			_productBalanceRepository = productBalanceRepository;
		}

		public async Task<BaseResult> Handle(DeleteSalesInvoiceCommand request, CancellationToken cancellationToken)
		{
			try
			{
				// الحصول على الفاتورة مع التفاصيل
				var entity = await _repository.GetWithDetailsByIdAsync(request.Id, cancellationToken);
				if (entity is null)
				{
					return new Error(ErrorCode.NotFound, _translator.GetString(TranslatorMessages.SalesInvoiceMessages.SalesInvoice_NotFound_with_id(request.Id)), nameof(request.Id));
				}
				
				if (entity.Status == SalesInvoiceStatus.Posted)
				{
					return new Error(ErrorCode.AccessDenied, _translator.GetString("SalesInvoice_Delete_NotAllowed_After_Post"), nameof(request.Id));
				}

				// للمبيعات العادية: إرجاع AvailableQty للكميات المحذوفة
				if (entity.Type != SalesInvoiceType.Distributor && entity.Details?.Any() == true)
				{
					foreach (var detail in entity.Details)
					{
						var productBalance = await _productBalanceRepository.GetByProductIdAsync(detail.ProductId, cancellationToken);
						if (productBalance != null)
						{
							// إرجاع AvailableQty للكمية المحذوفة
							productBalance.Adjust(0, detail.Quantity);
							_productBalanceRepository.Update(productBalance);
						}
					}
				}

				_repository.Delete(entity);
				await _unitOfWork.SaveChangesAsync();
				return BaseResult.Ok();
			}
			catch (System.Exception ex)
			{
				// في حالة الخطأ، التراجع عن جميع التغييرات
				await _unitOfWork.RollbackAsync();
				return new Error(ErrorCode.Exception, $"خطأ في حذف فاتورة المبيعات: {ex.Message}", nameof(request.Id));
			}
		}
	}
}



