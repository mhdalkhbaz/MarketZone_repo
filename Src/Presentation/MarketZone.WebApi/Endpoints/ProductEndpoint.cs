using System.Threading.Tasks;
using MarketZone.Application.Features.Products.Commands.CreateProduct;
using MarketZone.Application.Features.Products.Commands.DeleteProduct;
using MarketZone.Application.Features.Products.Commands.UpdateProduct;
using MarketZone.Application.Features.Products.Queries.GetPagedListProduct;
using MarketZone.Application.Features.Products.Queries.GetProductById;
using MarketZone.Application.Features.Products.Queries.GetProductAverageCost;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Products.DTOs;
using MarketZone.WebApi.Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace MarketZone.WebApi.Endpoints
{
    public class ProductEndpoint : EndpointGroupBase
    {
        public override void Map(RouteGroupBuilder builder)
        {
            builder.MapGet(GetPagedListProduct);

            builder.MapGet(GetProductById);

			builder.MapGet(GetProductAverageCost);

            builder.MapPost(CreateProduct).RequireAuthorization();

            builder.MapPut(UpdateProduct).RequireAuthorization();

            builder.MapDelete(DeleteProduct).RequireAuthorization();
        }

        async Task<PagedResponse<ProductDto>> GetPagedListProduct(IMediator mediator, [AsParameters] GetPagedListProductQuery model)
            => await mediator.Send<GetPagedListProductQuery, PagedResponse<ProductDto>>(model);

        async Task<BaseResult<ProductDto>> GetProductById(IMediator mediator, [AsParameters] GetProductByIdQuery model)
            => await mediator.Send<GetProductByIdQuery, BaseResult<ProductDto>>(model);

		async Task<BaseResult<decimal>> GetProductAverageCost(IMediator mediator, [AsParameters] GetProductAverageCostQuery model)
			=> await mediator.Send<GetProductAverageCostQuery, BaseResult<decimal>>(model);

        async Task<BaseResult<long>> CreateProduct(IMediator mediator, [FromBody] CreateProductCommand model)
            => await mediator.Send<CreateProductCommand, BaseResult<long>>(model);

        async Task<BaseResult> UpdateProduct(IMediator mediator, [FromBody] UpdateProductCommand model)
            => await mediator.Send<UpdateProductCommand, BaseResult>(model);

        async Task<BaseResult> DeleteProduct(IMediator mediator, [AsParameters] DeleteProductCommand model)
            => await mediator.Send<DeleteProductCommand, BaseResult>(model);

    }
}
