using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Helpers;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Suppliers.Commands.UpdateSupplier
{
	public class UpdateSupplierCommandHandler(ISupplierRepository supplierRepository, IUnitOfWork unitOfWork, ITranslator translator) : IRequestHandler<UpdateSupplierCommand, BaseResult>
	{
		public async Task<BaseResult> Handle(UpdateSupplierCommand request, CancellationToken cancellationToken)
		{
		var supplier = await supplierRepository.GetByIdAsync(request.Id);

		if (supplier is null)
		{
			return new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.SupplierMessages.Supplier_NotFound_with_id(request.Id)), nameof(request.Id));
		}

		supplier.Update(
			request.Name ?? supplier.Name,
			request.Phone ?? supplier.Phone,
			request.WhatsAppPhone ?? supplier.WhatsAppPhone,
			request.Email ?? supplier.Email,
			request.Address ?? supplier.Address,
			request.Currency ?? supplier.Currency,
			request.IsActive ?? supplier.IsActive
		);
		await unitOfWork.SaveChangesAsync();

			return BaseResult.Ok();
		}
	}
}



