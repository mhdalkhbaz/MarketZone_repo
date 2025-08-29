using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Suppliers.Entities;

namespace MarketZone.Application.Features.Suppliers.Commands.CreateSupplier
{
	public class CreateSupplierCommandHandler(ISupplierRepository supplierRepository, IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<CreateSupplierCommand, BaseResult<long>>
	{
		public async Task<BaseResult<long>> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
		{
			var supplier = mapper.Map<Supplier>(request);

			await supplierRepository.AddAsync(supplier);
			await unitOfWork.SaveChangesAsync();

			return supplier.Id;
		}
	}
}



