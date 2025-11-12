using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Sales.Entities;
using MarketZone.Domain.Sales.Enums;
using MarketZone.Application.Interfaces.Services;

namespace MarketZone.Application.Features.Sales.Commands.CreateSalesInvoice
{
	public class CreateSalesInvoiceCommandHandler : IRequestHandler<CreateSalesInvoiceCommand, BaseResult<long>>
	{
		private readonly ISalesInvoiceRepository _repository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
        private readonly IDistributionTripRepository _tripRepository;
        private readonly IInvoiceNumberGenerator _numberGenerator;
        private readonly IProductRepository _productRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IEmployeeSalaryRepository _employeeSalaryRepository;

		public CreateSalesInvoiceCommandHandler(
			ISalesInvoiceRepository repository,
			IUnitOfWork unitOfWork,
			IMapper mapper,
            IDistributionTripRepository tripRepository,
            IInvoiceNumberGenerator numberGenerator,
            IProductRepository productRepository,
            IEmployeeRepository employeeRepository,
            IEmployeeSalaryRepository employeeSalaryRepository)
		{
			_repository = repository;
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_tripRepository = tripRepository;
			_numberGenerator = numberGenerator;
            _productRepository = productRepository;
            _employeeRepository = employeeRepository;
            _employeeSalaryRepository = employeeSalaryRepository;
		}

		public async Task<BaseResult<long>> Handle(CreateSalesInvoiceCommand request, CancellationToken cancellationToken)
		{
			try
			{
				// Ensure invoice number is generated if not provided
                if (string.IsNullOrWhiteSpace(request.InvoiceNumber))
                {
                    request.InvoiceNumber = await _numberGenerator.GenerateAsync(MarketZone.Domain.Cash.Enums.InvoiceType.SalesInvoice, cancellationToken);
                }

				// Validate pricing: prevent zero/negative prices
				if (request.Details?.Any() == true)
				{
					if (request.Details.Any(d => d.UnitPrice <= 0))
						return new Error(ErrorCode.FieldDataInvalid, "Unit price must be greater than zero for all products", nameof(request.Details));

					// Recalculate subtotals based on validated prices
					foreach (var d in request.Details)
					{
						d.SubTotal = d.Quantity * d.UnitPrice;
					}

					// Recalculate invoice total amount prior to mapping to entity
					var computedTotal = request.Details.Sum(x => x.SubTotal) - request.Discount;
					request.TotalAmount = computedTotal < 0 ? 0 : computedTotal;
				}

				var invoice = _mapper.Map<SalesInvoice>(request);

				// إذا كانت فاتورة موزع، التحقق من رحلة التوزيع
				if (request.DistributionTripId.HasValue)
				{
					var trip = await _tripRepository.GetWithDetailsByIdAsync(request.DistributionTripId.Value, cancellationToken);
					if (trip == null)
						return new Error(ErrorCode.NotFound, "Distribution trip not found", nameof(request.DistributionTripId));

					// التحقق من أن رحلة التوزيع لم تكتمل بعد (Status <= 4)
					if ((short)trip.Status > 4)
						return new Error(ErrorCode.FieldDataInvalid, "Cannot create distributor invoice for completed or cancelled trip", nameof(request.DistributionTripId));

					// التحقق من أن رحلة التوزيع في حالة GoodsReceived أو أقل
					if (trip.Status != MarketZone.Domain.Logistics.Enums.DistributionTripStatus.GoodsReceived && 
						trip.Status != MarketZone.Domain.Logistics.Enums.DistributionTripStatus.InProgress)
						return new Error(ErrorCode.FieldDataInvalid, "Cannot create distributor invoice for trip that is not in GoodsReceived or InProgress status", nameof(request.DistributionTripId));

					// التحقق من أن الكمية لم تنتهي (خلصت الكمية)
					var hasRemainingQuantity = trip.Details.Any(d => (d.Qty - d.SoldQty - d.ReturnedQty) > 0);
					if (!hasRemainingQuantity)
						return new Error(ErrorCode.FieldDataInvalid, "Cannot create distributor invoice - all quantities have been sold or returned (خلصت الكمية)", nameof(request.DistributionTripId));

					invoice.SetDistributionTrip(trip);
					trip.AddSalesInvoice(invoice);

					// تحديث SoldQty في تفاصيل رحلة التوزيع
					if (request.Type == SalesInvoiceType.Distributor)
					{
						foreach (var detail in request.Details)
						{
							var tripDetail = trip.Details.FirstOrDefault(d => d.ProductId == detail.ProductId);
							if (tripDetail != null)
							{
								// التحقق من أن الكمية المباعة لا تتجاوز الكمية المحملة
								if (tripDetail.SoldQty + detail.Quantity > tripDetail.Qty)
									return new Error(ErrorCode.FieldDataInvalid, $"Sold quantity ({tripDetail.SoldQty + detail.Quantity}) cannot exceed loaded quantity ({tripDetail.Qty}) for product {detail.ProductId}", nameof(request.Details));

								tripDetail.AddSoldQty(detail.Quantity);
							}
						}

						// التحقق من أن جميع الكميات انتهت بعد البيع
						var allQuantitiesFinished = !trip.Details.Any(d => (d.Qty - d.SoldQty - d.ReturnedQty) > 0);
						if (allQuantitiesFinished)
						{
							// إذا انتهت جميع الكميات، تحديث حالة الرحلة إلى مكتملة
							trip.SetStatus(MarketZone.Domain.Logistics.Enums.DistributionTripStatus.Completed);

                            // حساب نسبة الموظف من عمولة المنتجات المباعة
                            var employee = await _employeeRepository.GetByIdAsync(trip.EmployeeId);
                            if (employee != null && employee.SalaryType == MarketZone.Domain.Employees.Enums.SalaryType.FixedWithPercentage && employee.SalaryPercentage.HasValue && employee.SalaryPercentage.Value > 0)
                            {
                                decimal totalPercentageAmount = 0;
                                var employeePercentage = employee.SalaryPercentage.Value / 100m;

                                foreach (var tripDetail in trip.Details)
                                {
                                    var netSoldQty = tripDetail.SoldQty - tripDetail.ReturnedQty;
                                    if (netSoldQty <= 0)
                                        continue;

                                    var product = await _productRepository.GetByIdAsync(tripDetail.ProductId);
                                    if (product == null || product.CommissionPerKg == null || product.CommissionPerKg <= 0)
                                        continue;

									totalPercentageAmount += (netSoldQty * product.CommissionPerKg.Value) * employeePercentage;
                                }

                              
                                    var currentYear = trip.TripDate.Year;
                                    var currentMonth = trip.TripDate.Month;

                                    var employeeSalary = await _employeeSalaryRepository.GetOrCreateAsync(
                                        employee.Id,
                                        currentYear,
                                        currentMonth,
                                        employee.Salary,
										totalPercentageAmount);

                                    if (employeeSalary.BaseSalary != employee.Salary)
                                    {
                                        employeeSalary.UpdateBaseSalary(employee.Salary);
                                    }

                                    employeeSalary.AddPercentageAmount(totalPercentageAmount);
                                    _employeeSalaryRepository.Update(employeeSalary);
                            }
						}
					}
				}

				// Save invoice first to get the ID
				await _repository.AddAsync(invoice);
				await _unitOfWork.SaveChangesAsync();

				// Add details after invoice is saved to get the correct ID
				if (request.Details?.Any() == true)
				{
					foreach (var d in request.Details)
					{
						var detail = _mapper.Map<SalesInvoiceDetail>(d);
						invoice.AddDetail(detail);
					}
				}
				
				// Save changes to persist the details
				await _unitOfWork.SaveChangesAsync();
				return invoice.Id;
			}
			catch (System.Exception ex)
			{
				// في حالة الخطأ، التراجع عن جميع التغييرات
				await _unitOfWork.RollbackAsync();
				return new Error(ErrorCode.Exception, $"Error creating sales invoice: {ex.Message}", nameof(request));
			}
		}
	}
}



