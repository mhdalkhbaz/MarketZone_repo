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

            CreateMap<CreateProductCommand, Product>();
                //.ConstructUsing(s => new Product(
                //    s.CategoryId.Value,
                //    s.Name,
                //    s.Description,
                //    s.UnitOfMeasure,
                //    s.MinStockLevel,
                //    s.IsActive,
                //    s.NeedsRoasting,
                //    s.BarCode))
                //.AfterMap((src, dest) =>
                //{
                //    dest.SetRawProduct(src.RawProductId);
                //    //dest.SetCommissionPerKg(src.CommissionPerKg);
                //});


            CreateMap<UpdateProductCommand, Product>();
                //.IgnoreAllPropertiesWithAnInaccessibleSetter()
                //.AfterMap((src, dest) =>
                //{
                //    // Use Product.Update() method to update properties properly
                //    dest.Update(
                //        src.CategoryId,
                //        src.Name ?? dest.Name,
                //        src.Description ?? dest.Description,
                //        src.UnitOfMeasure ?? dest.UnitOfMeasure,
                //        src.MinStockLevel ?? dest.MinStockLevel,
                //        src.IsActive ?? dest.IsActive,
                //        src.NeedsRoasting ?? dest.NeedsRoasting,
                //        src.BarCode ?? dest.BarCode);
                    
                //    // Update commission if provided
                //    if (src.CommissionPerKg.HasValue)
                //    {
                //        dest.SetCommissionPerKg(src.CommissionPerKg);
                //    }
                //});
        }
    }
}


