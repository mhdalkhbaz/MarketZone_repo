using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Logistics.Entities;
using MarketZone.Domain.Inventory.Entities;
using System.Collections.Generic;

namespace MarketZone.Application.Features.Logistics.Commands.DistributionTrips.CreateDistributionTrip
{
	public class CreateDistributionTripCommandHandler : IRequestHandler<CreateDistributionTripCommand, BaseResult<long>>
	{
		private readonly IDistributionTripRepository _repository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		private readonly IEmployeeRepository _employeeRepository;
		private readonly ICarRepository _carRepository;
		private readonly IRegionRepository _regionRepository;
		private readonly IProductRepository _productRepository;
		private readonly IProductBalanceRepository _productBalanceRepository;

		public CreateDistributionTripCommandHandler(
			IDistributionTripRepository repository,
			IUnitOfWork unitOfWork,
			IMapper mapper,
			IEmployeeRepository employeeRepository,
			ICarRepository carRepository,
			IRegionRepository regionRepository,
			IProductRepository productRepository,
			IProductBalanceRepository productBalanceRepository)
		{
			_repository = repository;
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_employeeRepository = employeeRepository;
			_carRepository = carRepository;
			_regionRepository = regionRepository;
			_productRepository = productRepository;
			_productBalanceRepository = productBalanceRepository;
		}

		public async Task<BaseResult<long>> Handle(CreateDistributionTripCommand request, CancellationToken cancellationToken)
		{
			try
			{
				// التحقق من وجود تفاصيل
				if (request.Details == null || !request.Details.Any())
					return new Error(ErrorCode.FieldDataInvalid, "At least one detail is required", nameof(request.Details));

				// التحقق من وجود الموظف
				var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId);
				if (employee == null)
					return new Error(ErrorCode.NotFound, "Employee not found", nameof(request.EmployeeId));

				// التحقق من وجود السيارة
				var car = await _carRepository.GetByIdAsync(request.CarId);
				if (car == null)
					return new Error(ErrorCode.NotFound, "Car not found", nameof(request.CarId));

				// التحقق من وجود المنطقة
				var region = await _regionRepository.GetByIdAsync(request.RegionId);
				if (region == null)
					return new Error(ErrorCode.NotFound, "Region not found", nameof(request.RegionId));

				// التحقق من وجود المنتجات والكميات المتاحة
				var productIds = request.Details.Select(d => d.ProductId).Distinct().ToList();
				var products = await _productRepository.GetByIdsAsync(productIds, cancellationToken);
				
				if (products.Count != productIds.Count)
				{
					var existingProductIds = products.Select(p => p.Key).ToList();
					var missingProductIds = productIds.Except(existingProductIds).ToList();
					return new Error(ErrorCode.NotFound, $"Products not found: {string.Join(", ", missingProductIds)}", nameof(request.Details));
				}

				// التحقق من الكميات المتاحة في ProductBalance
				var productBalances = new Dictionary<long, ProductBalance>();
				foreach (var detail in request.Details)
				{
					var balance = await _productBalanceRepository.GetByProductIdAsync(detail.ProductId, cancellationToken);
					if (balance == null)
						return new Error(ErrorCode.FieldDataInvalid, $"Product balance not found for product {detail.ProductId}", nameof(request.Details));

					if (balance.AvailableQty < detail.Qty)
						return new Error(ErrorCode.FieldDataInvalid, $"Insufficient available quantity for product {detail.ProductId}. Available: {balance.AvailableQty}, Requested: {detail.Qty}", nameof(request.Details));

					productBalances[detail.ProductId] = balance;
				}

				// إنشاء رحلة التوزيع
				var trip = _mapper.Map<DistributionTrip>(request);

				// إضافة التفاصيل
				if (request.Details?.Any() == true)
				{
					foreach (var d in request.Details)
					{
						var detail = new DistributionTripDetail(0, d.ProductId, d.Qty, d.ExpectedPrice);
						trip.AddDetail(detail);
					}
				}

				// حفظ رحلة التوزيع
				await _repository.AddAsync(trip);
				await _unitOfWork.SaveChangesAsync();

				// نقص الكميات المتاحة من ProductBalance
				foreach (var detail in request.Details)
				{
					var balance = productBalances[detail.ProductId];
					balance.Adjust(0, -detail.Qty); // نقص AvailableQty فقط
				}

				await _unitOfWork.SaveChangesAsync();

				return trip.Id;
			}
			catch (System.Exception ex)
			{
				// في حالة الخطأ، التراجع عن جميع التغييرات
				await _unitOfWork.RollbackAsync();
				return new Error(ErrorCode.Exception, $"Error creating distribution trip: {ex.Message}", nameof(request));
			}
		}
	}
}


