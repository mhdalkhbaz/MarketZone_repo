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
                    s.PurchasePrice,
                    s.SalePrice,
                    s.MinStockLevel,
                    s.IsActive,
                    s.NeedsRoasting,
                    s.RoastingCost,
                    s.BarCode))
                .AfterMap((src, dest) =>
                {
                    dest.SetRawProduct(src.RawProductId);
                    //dest.SetCommissionPerKg(src.CommissionPerKg);
                });


            //CreateMap<UpdateProductCommand, Product>()
            //    .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null))
            //    .AfterMap((src, dest) =>
            //    {
            //        dest.SetCommissionPerKg(src.CommissionPerKg);
            //    });
        }
    }
}


