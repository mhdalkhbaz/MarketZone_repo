using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Logistics.Enums;
using MarketZone.Domain.Employees.Enums;

namespace MarketZone.Application.Features.Logistics.Commands.DistributionTrips.ReceiveGoods
{
	public class ReceiveGoodsCommandHandler : IRequestHandler<ReceiveGoodsCommand, BaseResult>
	{
		private readonly IDistributionTripRepository _repository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IEmployeeRepository _employeeRepository;
		private readonly IEmployeeSalaryRepository _employeeSalaryRepository;
		private readonly IProductRepository _productRepository;
		private readonly ISalesInvoiceRepository _salesInvoiceRepository;

		public ReceiveGoodsCommandHandler(
			IDistributionTripRepository repository, 
			IUnitOfWork unitOfWork,
			IEmployeeRepository employeeRepository,
			IEmployeeSalaryRepository employeeSalaryRepository,
			IProductRepository productRepository,
			ISalesInvoiceRepository salesInvoiceRepository)
		{
			_repository = repository;
			_unitOfWork = unitOfWork;
			_employeeRepository = employeeRepository;
			_employeeSalaryRepository = employeeSalaryRepository;
			_productRepository = productRepository;
			_salesInvoiceRepository = salesInvoiceRepository;
		}

		public async Task<BaseResult> Handle(ReceiveGoodsCommand request, CancellationToken cancellationToken)
		{
			try
			{
				var trip = await _repository.GetWithDetailsByIdAsync(request.TripId, cancellationToken);
				if (trip is null)
					return new Error(ErrorCode.NotFound, "Distribution trip not found", nameof(request.TripId));

				// التحقق من أن الرحلة في حالة Posted
				if (trip.Status != DistributionTripStatus.Posted)
					return new Error(ErrorCode.FieldDataInvalid, "Cannot receive goods for trip that is not in Posted status", nameof(request.TripId));

				// التحقق من وجود التفاصيل
				if (request.Details == null || !request.Details.Any())
					return new Error(ErrorCode.FieldDataInvalid, "At least one detail is required", nameof(request.Details));

				// تحديث الكميات المرجعة
				foreach (var item in request.Details)
				{
					var detail = trip.Details.FirstOrDefault(d => d.Id == item.DetailId);
					if (detail == null)
						return new Error(ErrorCode.NotFound, $"Trip detail not found: {item.DetailId}", nameof(request.Details));

					// التحقق من أن الكمية المرجعة لا تتجاوز الكمية المحملة
					if (item.ReturnedQty > detail.Qty)
						return new Error(ErrorCode.FieldDataInvalid, $"Returned quantity ({item.ReturnedQty}) cannot exceed loaded quantity ({detail.Qty}) for detail {item.DetailId}", nameof(request.Details));

					detail.UpdateReturnedQty(item.ReturnedQty);
				}

				// تغيير حالة الرحلة إلى GoodsReceived
				trip.SetStatus(DistributionTripStatus.GoodsReceived);

				// حساب نسبة الموظف من عمولة المنتجات المباعة
				var employee = await _employeeRepository.GetByIdAsync(trip.EmployeeId);
				if (employee != null && employee.SalaryType == SalaryType.FixedWithPercentage && employee.SalaryPercentage.HasValue)
				{
					await CalculateAndAddPercentageSalary(trip, employee, cancellationToken);
				}

				await _unitOfWork.SaveChangesAsync();
				return BaseResult.Ok();
			}
			catch (System.Exception ex)
			{
				// في حالة الخطأ، التراجع عن جميع التغييرات
				await _unitOfWork.RollbackAsync();
				return new Error(ErrorCode.Exception, $"Error receiving goods: {ex.Message}", nameof(request.TripId));
			}
		}

		/// <summary>
		/// حساب النسبة من عمولة المنتجات المباعة (SoldQty - ReturnedQty) للموظف وإضافتها لـ EmployeeSalary
		/// الحساب: (كمية المنتج المباع - المرتجع) * CommissionPerKg * (Employee.SalaryPercentage / 100)
		/// </summary>
		private async Task CalculateAndAddPercentageSalary(Domain.Logistics.Entities.DistributionTrip trip, Domain.Employees.Entities.Employee employee, CancellationToken cancellationToken)
		{
			// جلب جميع فواتير المبيعات المرتبطة بالرحلة والموافقة (Posted)
			var salesInvoices = await _salesInvoiceRepository.GetInvoicesByTripIdAsync(trip.Id, cancellationToken);
			
			if (!salesInvoices.Any())
				return; // لا توجد فواتير مرتبطة بالرحلة

			decimal totalPercentageAmount = 0;
			var employeePercentage = employee.SalaryPercentage.Value / 100m; // تحويل النسبة إلى عشري

			// حساب الكمية المباعة والمرتجعة لكل منتج
			var productSoldQuantities = new System.Collections.Generic.Dictionary<long, decimal>();
			var productReturnedQuantities = new System.Collections.Generic.Dictionary<long, decimal>();

			// جمع الكميات المباعة من الفواتير المؤكدة
			foreach (var invoice in salesInvoices)
			{
				if (invoice.Status != Domain.Sales.Enums.SalesInvoiceStatus.Posted)
					continue; // نحسب فقط من الفواتير المؤكدة

				if (invoice.Details == null || !invoice.Details.Any())
					continue;

				foreach (var detail in invoice.Details)
				{
					if (!productSoldQuantities.ContainsKey(detail.ProductId))
						productSoldQuantities[detail.ProductId] = 0;
					
					productSoldQuantities[detail.ProductId] += detail.Quantity;
				}
			}

			// جمع الكميات المرتجعة من تفاصيل الرحلة
			if (trip.Details != null && trip.Details.Any())
			{
				foreach (var detail in trip.Details)
				{
					productReturnedQuantities[detail.ProductId] = detail.ReturnedQty;
				}
			}

			// حساب النسبة من عمولة كل منتج مباع
			foreach (var productId in productSoldQuantities.Keys)
			{
				var soldQty = productSoldQuantities[productId];
				var returnedQty = productReturnedQuantities.ContainsKey(productId) ? productReturnedQuantities[productId] : 0;
				
				// الكمية المباعة فعلياً = الكمية المباعة - المرتجع
				var netSoldQty = soldQty - returnedQty;
				if (netSoldQty <= 0)
					continue; // لا يوجد بيع فعلي

				// جلب المنتج للحصول على CommissionPerKg
				var product = await _productRepository.GetByIdAsync(productId);
				if (product == null || product.CommissionPerKg == null || product.CommissionPerKg <= 0)
					continue; // لا يوجد عمولة للمنتج

				// الحساب: (كمية المنتج المباع - المرتجع) * CommissionPerKg * (Employee.SalaryPercentage / 100)
				var percentageAmount = netSoldQty * product.CommissionPerKg.Value * employeePercentage;
				totalPercentageAmount += percentageAmount;
			}

			if (totalPercentageAmount > 0)
			{
				// إنشاء أو الحصول على EmployeeSalary للشهر الحالي
				var currentYear = trip.TripDate.Year;
				var currentMonth = trip.TripDate.Month;

				var employeeSalary = await _employeeSalaryRepository.GetOrCreateAsync(
					employee.Id,
					currentYear,
					currentMonth,
					employee.Salary);

				// إضافة مبلغ النسبة
				employeeSalary.AddPercentageAmount(totalPercentageAmount);
				_employeeSalaryRepository.Update(employeeSalary);
			}
		}
	}
}
