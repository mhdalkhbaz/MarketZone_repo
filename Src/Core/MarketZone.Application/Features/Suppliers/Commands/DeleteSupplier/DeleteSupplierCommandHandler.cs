using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Helpers;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Suppliers.Commands.DeleteSupplier
{
	public class DeleteSupplierCommandHandler(ISupplierRepository supplierRepository, IUnitOfWork unitOfWork, ITranslator translator) : IRequestHandler<DeleteSupplierCommand, BaseResult>
	{
		public async Task<BaseResult> Handle(DeleteSupplierCommand request, CancellationToken cancellationToken)
		{
			var supplier = await supplierRepository.GetByIdAsync(request.Id);

			if (supplier is null)
			{
				return new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.SupplierMessages.Supplier_NotFound_with_id(request.Id)), nameof(request.Id));
			}

			supplierRepository.Delete(supplier);
			await unitOfWork.SaveChangesAsync();

			return BaseResult.Ok();
		}
	}
}



