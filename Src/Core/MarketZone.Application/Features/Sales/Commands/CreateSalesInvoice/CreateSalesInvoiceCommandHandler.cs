using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Application.Interfaces.Services;
using MarketZone.Domain.Sales.Enums;
using MarketZone.Domain.Sales.Entities;

namespace MarketZone.Application.Features.Sales.Commands.CreateSalesInvoice
{
	public class CreateSalesInvoiceCommandHandler(ISalesInvoiceRepository repository, IUnitOfWork unitOfWork, IMapper mapper, ISalesInvoiceNumberGenerator numberGenerator) : IRequestHandler<CreateSalesInvoiceCommand, BaseResult<long>>
	{
		public async Task<BaseResult<long>> Handle(CreateSalesInvoiceCommand request, CancellationToken cancellationToken)
		{
			if (request.Details == null || !request.Details.Any())
				return new Error(ErrorCode.FieldDataInvalid, "At least one line is required", nameof(request.Details));
			var invoice = mapper.Map<SalesInvoice>(request);
			// default status
			// set number if empty
			if (string.IsNullOrWhiteSpace(invoice.InvoiceNumber))
			{
				invoice.GetType().GetProperty(nameof(invoice.InvoiceNumber))!.SetValue(invoice, await numberGenerator.GenerateAsync(cancellationToken));
			}

			if (request.Details?.Any() == true)
			{
				foreach (var d in request.Details)
				{
					invoice.Details.Add(new SalesInvoiceDetail(0, d.ProductId, d.Quantity, d.UnitPrice, d.SubTotal, d.Notes));
				}
			}

			await repository.AddAsync(invoice);
			await unitOfWork.SaveChangesAsync();

			return invoice.Id;
		}
	}
}



