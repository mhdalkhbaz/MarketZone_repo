using AutoMapper;
using MarketZone.Application.Features.Products.Commands.CreateProduct;
using MarketZone.Application.Features.Products.Commands.UpdateProduct;
using MarketZone.Domain.Products.DTOs;
using MarketZone.Domain.Products.Entities;

namespace MarketZone.Application.Features.Products
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductDto>()
                .ForMember(d => d.CreatedDateTime, o => o.MapFrom(s => s.Created));

            CreateMap<CreateProductCommand, Product>()
                .ConstructUsing(s => new Product(
                    s.CategoryId.Value,
                    s.Name,
                    s.Description,
                    s.UnitOfMeasure,
                    s.MinStockLevel,
                    s.IsActive,
                    s.NeedsRoasting,
                    s.BarCode,
                    s.ProductType))
                .AfterMap((src, dest) =>
                {
                    dest.SetRawProduct(src.RawProductId);
                    dest.SetCommissionPerKg(src.CommissionPerKg);
                });


            CreateMap<UpdateProductCommand, Product>()
                .ForMember(dest => dest.ProductType, opt => opt.Ignore());

            // Composite Product mappings
            CreateMap<CompositeProduct, CompositeProductDto>()
                .ForMember(d => d.CreatedDateTime, o => o.MapFrom(s => s.Created))
                .ForMember(d => d.ResultingProductName, o => o.MapFrom(s => s.ResultingProduct != null ? s.ResultingProduct.Name : string.Empty))
                .ForMember(d => d.Details, o => o.MapFrom(s => s.Details));

            CreateMap<CompositeProductDetail, CompositeProductDetailDto>()
                .ForMember(d => d.ComponentProductName, o => o.MapFrom(s => s.ComponentProduct != null ? s.ComponentProduct.Name : string.Empty));
        }
    }
}


