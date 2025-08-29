using System;
using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Roasting.Enums;
using MarketZone.Domain.Inventory.Entities;

namespace MarketZone.Application.Features.Roasting.Commands.DeleteRoastingInvoice
{
    public class DeleteRoastingInvoiceCommandHandler : IRequestHandler<DeleteRoastingInvoiceCommand, BaseResult>
    {
        private readonly IRoastingInvoiceRepository _repository;
        private readonly IUnroastedProdcutBalanceRepository _unroastedRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteRoastingInvoiceCommandHandler(
            IRoastingInvoiceRepository repository,
            IUnroastedProdcutBalanceRepository unroastedRepository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unroastedRepository = unroastedRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResult> Handle(DeleteRoastingInvoiceCommand request, CancellationToken cancellationToken)
        {
            var roastingInvoice = await _repository.GetWithDetailsByIdAsync(request.Id);
            if (roastingInvoice == null)
            {
                throw new InvalidOperationException($"Roasting invoice with ID {request.Id} not found.");
            }

            // Prevent deletion if invoice is posted
            if (roastingInvoice.Status == RoastingInvoiceStatus.Posted)
            {
                throw new InvalidOperationException("Cannot delete a posted roasting invoice.");
            }

            // Release all reserved quantities
            foreach (var detail in roastingInvoice.Details)
            {
                var unroastedBalance = await _unroastedRepository.GetByProductIdAsync(detail.ProductId, cancellationToken);
                if (unroastedBalance != null)
                {
                    unroastedBalance.Release(detail.QuantityKg);
                    _unroastedRepository.Update(unroastedBalance);
                }
            }

            _repository.Delete(roastingInvoice);
            await _unitOfWork.SaveChangesAsync();

            return new BaseResult { Success = true };
        }
    }
}
