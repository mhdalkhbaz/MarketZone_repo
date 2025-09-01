using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MarketZone.Application.Features.Sales.Commands.CreateSalesInvoice;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Logistics.Enums;

namespace MarketZone.Application.Features.Logistics.Commands.DistributionTrips.CreateSalesInvoices
{
	public class CreateSalesInvoicesFromTripCommandHandler(
		IDistributionTripRepository tripRepository,
		ISalesInvoiceRepository salesInvoiceRepository,
		IUnitOfWork unitOfWork,
		IMapper mapper) : IRequestHandler<CreateSalesInvoicesFromTripCommand, BaseResult<List<long>>>
	{
		public async Task<BaseResult<List<long>>> Handle(CreateSalesInvoicesFromTripCommand request, CancellationToken cancellationToken)
		{
			var trip = await tripRepository.GetWithDetailsByIdAsync(request.TripId, cancellationToken);
			if (trip is null)
				return BaseResult<List<long>>.Failure();

			if (trip.Status != DistributionTripStatus.GoodsReceived)
				return BaseResult<List<long>>.Failure();

			var createdInvoiceIds = new List<long>();

			foreach (var customerSale in request.CustomerSales)
			{
				// إنشاء فاتورة مبيعات للعميل
				var salesInvoiceCommand = new CreateSalesInvoiceCommand
				{
					CustomerId = customerSale.CustomerId,
					InvoiceDate = trip.TripDate,
					PaymentMethod = "Distributor", // نوع الدفع: موزع
					Notes = $"فاتورة توزيع - رحلة رقم {trip.Id}",
					Details = customerSale.Details.Select(d => new CreateSalesInvoiceDetailItem
					{
						ProductId = d.ProductId,
						Quantity = d.Quantity,
						UnitPrice = d.UnitPrice,
						SubTotal = d.SubTotal,
						Notes = d.Notes
					}).ToList()
				};

				// حساب المجاميع
				salesInvoiceCommand.TotalAmount = customerSale.Details.Sum(d => d.SubTotal);
				salesInvoiceCommand.Discount = 0;

				// إنشاء الفاتورة
				var invoice = mapper.Map<MarketZone.Domain.Sales.Entities.SalesInvoice>(salesInvoiceCommand);
				await salesInvoiceRepository.AddAsync(invoice);
				createdInvoiceIds.Add(invoice.Id);
			}

			await unitOfWork.SaveChangesAsync();
			return createdInvoiceIds;
		}
	}
}
