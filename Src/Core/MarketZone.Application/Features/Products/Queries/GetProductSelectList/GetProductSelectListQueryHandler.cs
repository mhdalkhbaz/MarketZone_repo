using MarketZone.Application.DTOs;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Products.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Helpers;

namespace MarketZone.Application.Features.Products.Queries.GetProductSelectList
{
    public class GetProductSelectListQueryHandler : IRequestHandler<GetProductSelectListQuery, BaseResult<List<ProductSelectListDto>>>
    {
	private readonly IDistributionTripRepository _tripRepository;
	private readonly IProductRepository _productRepository;
	private readonly IProductBalanceRepository _productBalanceRepository;
	private readonly ITranslator _translator;

	public GetProductSelectListQueryHandler(
		IDistributionTripRepository tripRepository,
		IProductRepository productRepository,
		IProductBalanceRepository productBalanceRepository,
		ITranslator translator)
	{
		_tripRepository = tripRepository;
		_productRepository = productRepository;
		_productBalanceRepository = productBalanceRepository;
		_translator = translator;
	}

        public async Task<BaseResult<List<ProductSelectListDto>>> Handle(GetProductSelectListQuery request, CancellationToken cancellationToken)
        {
            try
            {
                List<ProductSelectListDto> result = new List<ProductSelectListDto>();

                if (request.DistributionTripId.HasValue)
                {
                    // الحالة الأولى: إرجاع المنتجات المتبقية في رحلة التوزيع
                    result = await GetRemainingProductsForTrip(request.DistributionTripId.Value, cancellationToken);
                }
                else
                {
                    // الحالة الثانية: إرجاع المنتجات الجاهزة للبيع (المحمصة)
                    result = await GetReadyProductsForSale(cancellationToken);
                }

                return BaseResult<List<ProductSelectListDto>>.Ok(result);
            }
	catch (System.Exception ex)
	{
		var message = _translator.GetString(new TranslatorMessageDto("Error_Getting_Product_Select_List", new[] { ex.Message }));
		return new Error(ErrorCode.Exception, message, nameof(request));
	}
        }

        private async Task<List<ProductSelectListDto>> GetRemainingProductsForTrip(long tripId, CancellationToken cancellationToken)
        {
            // الحصول على رحلة التوزيع مع التفاصيل
            var trip = await _tripRepository.GetWithDetailsByIdAsync(tripId, cancellationToken);
            if (trip == null)
                return new List<ProductSelectListDto>();

            var result = new List<ProductSelectListDto>();

            foreach (var detail in trip.Details)
            {
                // حساب الكمية المتبقية = الكمية المحملة - الكمية المباعة - الكمية المرجعة
                var remainingQty = detail.Qty - detail.SoldQty - detail.ReturnedQty;
                
                if (remainingQty > 0)
                {
                    decimal price = detail.ExpectedPrice;

                    // If ExpectedPrice is 0, try to get the current SalePrice from ProductBalance
                    if (price == 0)
                    {
                        var balance = await _productBalanceRepository.GetByProductIdAsync(detail.ProductId, cancellationToken);
                        if (balance != null)
                        {
                            price = balance.SalePrice;
                        }
                    }

                    result.Add(new ProductSelectListDto(
                        detail.ProductId.ToString(),
                        detail.Product?.Name  ,
						remainingQty,
						price
                    ));
                }
            }

            return result.OrderBy(p => p.Label).ToList();
        }

        private async Task<List<ProductSelectListDto>> GetReadyProductsForSale(CancellationToken cancellationToken)
        {
            var productBalances = await _productBalanceRepository.GetAllProductBalanceAsync();

            var result = productBalances
                .Where(x => x.AvailableQty > 0 && x.Product.IsActive && !x.Product.NeedsRoasting)
                .Select(x => new ProductSelectListDto(
                    x.Product.Id.ToString(),
                    x.Product.Name,
                    x.AvailableQty,
                    x.SalePrice
                ))
                .OrderBy(p => p.Value)
                .ToList();

            return result;


        }
    }
}
