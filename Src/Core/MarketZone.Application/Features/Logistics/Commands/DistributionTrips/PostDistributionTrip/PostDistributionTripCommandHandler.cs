using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Logistics.Enums;
using MarketZone.Domain.Inventory.Entities;
using MarketZone.Domain.Employees.Enums;

namespace MarketZone.Application.Features.Logistics.Commands.DistributionTrips.PostDistributionTrip
{
	public class PostDistributionTripCommandHandler : IRequestHandler<PostDistributionTripCommand, BaseResult>
	{
		private readonly IDistributionTripRepository _repository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IProductBalanceRepository _productBalanceRepository;
		private readonly IEmployeeRepository _employeeRepository;
		private readonly IEmployeeSalaryRepository _employeeSalaryRepository;
		private readonly ISalesInvoiceRepository _salesInvoiceRepository;

		public PostDistributionTripCommandHandler(
			IDistributionTripRepository repository,
			IUnitOfWork unitOfWork,
			IProductBalanceRepository productBalanceRepository,
			IEmployeeRepository employeeRepository,
			IEmployeeSalaryRepository employeeSalaryRepository,
			ISalesInvoiceRepository salesInvoiceRepository)
		{
			_repository = repository;
			_unitOfWork = unitOfWork;
			_productBalanceRepository = productBalanceRepository;
			_employeeRepository = employeeRepository;
			_employeeSalaryRepository = employeeSalaryRepository;
			_salesInvoiceRepository = salesInvoiceRepository;
		}

		public async Task<BaseResult> Handle(PostDistributionTripCommand request, CancellationToken cancellationToken)
		{
			try
			{
				var trip = await _repository.GetWithDetailsByIdAsync(request.Id, cancellationToken);
				if (trip is null)
					return new Error(ErrorCode.NotFound, "Distribution trip not found", nameof(request.Id));

				if (trip.Status == DistributionTripStatus.Posted)
					return BaseResult.Ok();

				// التحقق من أن الرحلة في حالة Draft
				if (trip.Status != DistributionTripStatus.Draft)
					return new Error(ErrorCode.FieldDataInvalid, "Cannot post trip that is not in Draft status", nameof(request.Id));

                // لا تأثير على المخزون في مرحلة Post حسب المتطلبات الجديدة

				// تغيير حالة الرحلة إلى Posted
				trip.SetStatus(DistributionTripStatus.Posted);

				// حساب النسبة من عمولة المنتجات للموظف إذا كان له راتب "ثابت + نسبة"
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
				return new Error(ErrorCode.Exception, $"Error posting distribution trip: {ex.Message}", nameof(request.Id));
			}
		}

		/// <summary>
		/// حساب النسبة من عمولة المنتجات في الفواتير المرتبطة بالرحلة وإضافتها لـ EmployeeSalary
		/// </summary>
		private async Task CalculateAndAddPercentageSalary(Domain.Logistics.Entities.DistributionTrip trip, Domain.Employees.Entities.Employee employee, CancellationToken cancellationToken)
		{
			// الحصول على جميع فواتير المبيعات المرتبطة بالرحلة والموافقة (Posted)
			var salesInvoices = await _salesInvoiceRepository.GetInvoicesByTripIdAsync(trip.Id, cancellationToken);

			if (!salesInvoices.Any())
				return; // لا توجد فواتير مرتبطة بالرحلة

			decimal totalPercentageAmount = 0;
			var employeePercentage = employee.SalaryPercentage.Value / 100m; // تحويل النسبة إلى عشري

			// حساب النسبة من عمولة كل منتج في كل فاتورة
			foreach (var invoice in salesInvoices)
			{
				if (invoice.Status != Domain.Sales.Enums.SalesInvoiceStatus.Posted)
					continue; // نحسب فقط من الفواتير المؤكدة

				if (invoice.Details == null || !invoice.Details.Any())
					continue;

				foreach (var detail in invoice.Details)
				{
					if (detail.Product == null)
						continue;

					// كل منتج له عمولة خاصة
					var commissionPerKg = detail.Product.CommissionPerKg ?? 0;
					if (commissionPerKg > 0)
					{
						// الحساب: CommissionPerKg * Quantity * (Employee.SalaryPercentage / 100)
						var percentageAmount = commissionPerKg * detail.Quantity * employeePercentage;
						totalPercentageAmount += percentageAmount;
					}
				}
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
