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
using MarketZone.Application.DTOs;

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
	private readonly ITranslator _translator;

	public CreateDistributionTripCommandHandler(
		IDistributionTripRepository repository,
		IUnitOfWork unitOfWork,
		IMapper mapper,
		IEmployeeRepository employeeRepository,
		ICarRepository carRepository,
		IRegionRepository regionRepository,
		IProductRepository productRepository,
		IProductBalanceRepository productBalanceRepository,
		ITranslator translator)
	{
		_repository = repository;
		_unitOfWork = unitOfWork;
		_mapper = mapper;
		_employeeRepository = employeeRepository;
		_carRepository = carRepository;
		_regionRepository = regionRepository;
		_productRepository = productRepository;
		_productBalanceRepository = productBalanceRepository;
		_translator = translator;
	}

		public async Task<BaseResult<long>> Handle(CreateDistributionTripCommand request, CancellationToken cancellationToken)
		{
			try
			{
		// التحقق من وجود تفاصيل
		if (request.Details == null || !request.Details.Any())
			return new Error(ErrorCode.FieldDataInvalid, _translator.GetString("At_Least_One_Detail_Required"), nameof(request.Details));

		// التحقق من وجود الموظف
		var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId);
		if (employee == null)
			return new Error(ErrorCode.NotFound, _translator.GetString("Employee_Not_Found"), nameof(request.EmployeeId));

		// التحقق من وجود السيارة
		var car = await _carRepository.GetByIdAsync(request.CarId);
		if (car == null)
			return new Error(ErrorCode.NotFound, _translator.GetString("Car_Not_Found"), nameof(request.CarId));

		// التحقق من وجود المنطقة
		var region = await _regionRepository.GetByIdAsync(request.RegionId);
		if (region == null)
			return new Error(ErrorCode.NotFound, _translator.GetString("Region_Not_Found"), nameof(request.RegionId));

				// التحقق من وجود المنتجات والكميات المتاحة
				var productIds = request.Details.Select(d => d.ProductId).Distinct().ToList();
				var products = await _productRepository.GetByIdsAsync(productIds, cancellationToken);
				
		if (products.Count != productIds.Count)
		{
			var existingProductIds = products.Select(p => p.Key).ToList();
			var missingProductIds = productIds.Except(existingProductIds).ToList();
			var message = _translator.GetString(new TranslatorMessageDto("Products_Not_Found", new[] { string.Join(", ", missingProductIds) }));
			return new Error(ErrorCode.NotFound, message, nameof(request.Details));
		}

				// التحقق من الكميات المتاحة في ProductBalance
				var productBalances = new Dictionary<long, ProductBalance>();
		foreach (var detail in request.Details)
		{
			var balance = await _productBalanceRepository.GetByProductIdAsync(detail.ProductId, cancellationToken);
			if (balance == null)
			{
				var message = _translator.GetString(new TranslatorMessageDto("Product_Balance_Not_Found_For_Product", new[] { detail.ProductId.ToString() }));
				return new Error(ErrorCode.FieldDataInvalid, message, nameof(request.Details));
			}

			if (balance.AvailableQty < detail.Qty)
			{
				var message = _translator.GetString(new TranslatorMessageDto("Insufficient_Available_Quantity_For_Product", 
					new[] { detail.ProductId.ToString(), balance.AvailableQty.ToString(), detail.Qty.ToString() }));
				return new Error(ErrorCode.FieldDataInvalid, message, nameof(request.Details));
			}

			productBalances[detail.ProductId] = balance;
		}

				// إنشاء رحلة التوزيع
				var trip = _mapper.Map<DistributionTrip>(request);

				// إضافة التفاصيل
				if (request.Details?.Any() == true)
				{
					foreach (var d in request.Details)
					{
						var detail = new DistributionTripDetail(0, d.ProductId, d.Qty, d.ExpectedPrice.Value);
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
		var message = _translator.GetString(new TranslatorMessageDto("Error_Creating_Distribution_Trip", new[] { ex.Message }));
		return new Error(ErrorCode.Exception, message, nameof(request));
	}
		}
	}
}


