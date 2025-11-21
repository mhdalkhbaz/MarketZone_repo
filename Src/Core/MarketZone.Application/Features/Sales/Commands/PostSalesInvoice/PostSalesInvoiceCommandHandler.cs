using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Sales.Enums;
using MarketZone.Application.Interfaces.Services;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using MarketZone.Domain.Logistics.Enums;
using MarketZone.Domain.Employees.Enums;
using MarketZone.Domain.Inventory.Entities;

namespace MarketZone.Application.Features.Sales.Commands.PostSalesInvoice
{
    public class PostSalesInvoiceCommandHandler : IRequestHandler<PostSalesInvoiceCommand, BaseResult>
    {
        private readonly ISalesInvoiceRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITranslator _translator;
        private readonly ISalesInventoryService _inventoryService;
        private readonly IDistributionTripRepository _tripRepository;
        private readonly IProductBalanceRepository _productBalanceRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IEmployeeSalaryRepository _employeeSalaryRepository;
        private readonly IProductRepository _productRepository;
        private readonly IExchangeRateRepository _exchangeRateRepository;

        public PostSalesInvoiceCommandHandler(
            ISalesInvoiceRepository repository,
            IUnitOfWork unitOfWork,
            ITranslator translator,
            ISalesInventoryService inventoryService,
            IDistributionTripRepository tripRepository,
            IProductBalanceRepository productBalanceRepository,
            IEmployeeRepository employeeRepository,
            IEmployeeSalaryRepository employeeSalaryRepository,
            IProductRepository productRepository,
            IExchangeRateRepository exchangeRateRepository)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _translator = translator;
            _inventoryService = inventoryService;
            _tripRepository = tripRepository;
            _productBalanceRepository = productBalanceRepository;
            _employeeRepository = employeeRepository;
            _employeeSalaryRepository = employeeSalaryRepository;
            _productRepository = productRepository;
            _exchangeRateRepository = exchangeRateRepository;
        }

        public async Task<BaseResult> Handle(PostSalesInvoiceCommand request, CancellationToken cancellationToken)
        {
            var invoice = await _repository.GetWithDetailsByIdAsync(request.Id, cancellationToken);
            if (invoice is null)
                return new Error(ErrorCode.NotFound, _translator.GetString("SalesInvoice_NotFound_with_id"), nameof(request.Id));
            if (invoice.Status == SalesInvoiceStatus.Posted)
                return new Error(ErrorCode.ModelStateNotValid, _translator.GetString("PostedInvoice"), nameof(request.Id));

            // تطبيق التأثيرات على المخزون للمبيعات العادية
            await _inventoryService.ApplyOnPostAsync(invoice, cancellationToken);

            // إذا كانت فاتورة موزع، التحقق من رحلة التوزيع وإكمالها إذا لزم الأمر
            if (invoice.Type == SalesInvoiceType.Distributor && invoice.DistributionTripId.HasValue)
            {
                var trip = await _tripRepository.GetWithDetailsByIdAsync(invoice.DistributionTripId.Value, cancellationToken);
                if (trip == null)
                    return new Error(ErrorCode.NotFound, "رحلة التوزيع غير موجودة", nameof(invoice.DistributionTripId));

                // حساب نسبة الموظف من عمولة المنتجات المباعة في هذه الفاتورة (عند كل Post فاتورة)
                var employee = await _employeeRepository.GetByIdAsync(trip.EmployeeId);
                if (employee != null && employee.SalaryType == SalaryType.FixedWithPercentage && employee.SalaryPercentage.HasValue && employee.SalaryPercentage.Value > 0)
                {
                    decimal totalPercentageAmount = 0;
                    var employeePercentage = employee.SalaryPercentage.Value / 100m;

                    // حساب العمولة من تفاصيل الفاتورة الحالية (الكمية المباعة في هذه الفاتورة فقط)
                    if (invoice.Details != null && invoice.Details.Any())
                    {
                        foreach (var invoiceDetail in invoice.Details)
                        {
                            var product = await _productRepository.GetByIdAsync(invoiceDetail.ProductId);
                            if (product == null || product.CommissionPerKg == null || product.CommissionPerKg <= 0)
                                continue;

                            // حساب العمولة على الكمية المباعة في هذه الفاتورة
                            var commissionForThisInvoice = invoiceDetail.Quantity * product.CommissionPerKg.Value;
                            totalPercentageAmount += commissionForThisInvoice * employeePercentage;
                        }
                    }

                    if (totalPercentageAmount > 0)
                    {
                        // الحصول على سعر الصرف الحالي
                        var exchangeRate = await _exchangeRateRepository.GetLatestActiveRateAsync(cancellationToken);
                        if (exchangeRate == null)
                            return new Error(ErrorCode.FieldDataInvalid, "لا يوجد سعر صرف نشط", nameof(request.Id));

                        // ضرب totalPercentageAmount بسعر الصرف الحالي
                        var totalPercentageAmountInSyrian = totalPercentageAmount * exchangeRate.Rate;

                        var currentYear = trip.TripDate.Year;
                        var currentMonth = trip.TripDate.Month;

                        var employeeSalary = await _employeeSalaryRepository.GetOrCreateAsync(
                            employee.Id,
                            currentYear,
                            currentMonth,
                            employee.Salary,
                            null);

                        if (employeeSalary.BaseSalary != employee.Salary)
                        {
                            employeeSalary.UpdateBaseSalary(employee.Salary);
                        }

                        employeeSalary.AddPercentageAmount(totalPercentageAmountInSyrian);
                        _employeeSalaryRepository.Update(employeeSalary);
                    }
                }

                // التحقق من أن جميع الكميات انتهت بعد البيع
                var allQuantitiesFinished = !trip.Details.Any(d => (d.Qty - d.SoldQty - d.ReturnedQty) > 0);
                if (allQuantitiesFinished)
                {
                    // إذا انتهت جميع الكميات، تحديث حالة الرحلة إلى مكتملة
                    trip.SetStatus(DistributionTripStatus.Completed);

                    //// تطبيق التأثيرات على المخزون عند إكمال الرحلة
                    //// 1) إرجاع المرتجع إلى AvailableQty
                    //// 2) إنقاص المباع من Qty
                    //foreach (var tripDetail in trip.Details)
                    //{
                    //    var balance = await _productBalanceRepository.GetByProductIdAsync(tripDetail.ProductId, cancellationToken);
                    //    if (balance == null)
                    //        return new Error(ErrorCode.FieldDataInvalid, $"Product balance not found for product {tripDetail.ProductId}", nameof(invoice.DistributionTripId));

                    //    // 1) إرجاع المرتجع إلى AvailableQty
                    //    if (tripDetail.ReturnedQty > 0)
                    //    {
                    //        balance.Adjust(0, tripDetail.ReturnedQty); // زيادة المتاح فقط
                    //    }

                    //    // 2) إنقاص المباع من Qty
                    //    if (tripDetail.SoldQty > 0)
                    //    {
                    //        if (balance.Qty < tripDetail.SoldQty)
                    //            return new Error(ErrorCode.FieldDataInvalid, $"Insufficient quantity for product {tripDetail.ProductId}. Qty: {balance.Qty}, Sold: {tripDetail.SoldQty}", nameof(invoice.DistributionTripId));

                    //        balance.Adjust(-tripDetail.SoldQty, 0); // إنقاص الموجودة فقط
                    //    }

                    //    _productBalanceRepository.Update(balance);
                    //}
                }
            }

            invoice.SetStatus(SalesInvoiceStatus.Posted);
            await _unitOfWork.SaveChangesAsync();
            return BaseResult.Ok();
        }
    }
}


