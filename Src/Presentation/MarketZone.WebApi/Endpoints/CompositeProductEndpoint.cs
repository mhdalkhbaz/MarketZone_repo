using System.Collections.Generic;
using System.Threading.Tasks;
using MarketZone.Application.Features.Products.Commands.CreateCompositeProduct;
using MarketZone.Application.Features.Products.Commands.DeleteCompositeProduct;
using MarketZone.Application.Features.Products.Commands.UpdateCompositeProduct;
using MarketZone.Application.Features.Products.Commands.PostCompositeProduct;
using MarketZone.Application.Features.Products.Queries.GetProductsForComposite;
using MarketZone.Application.Features.Products.Queries.GetPagedListCompositeProduct;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Products.DTOs;
using MarketZone.WebApi.Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MarketZone.Application.Features.Products.Queries.GetProductById;

namespace MarketZone.WebApi.Endpoints
{
    public class CompositeProductEndpoint : EndpointGroupBase
    {
        public override void Map(RouteGroupBuilder builder)
        {
            builder.MapGet(GetPagedListCompositeProduct);
            builder.MapGet(GetProductsForComposite);

            builder.MapPost(CreateCompositeProduct).RequireAuthorization();

            builder.MapPut(UpdateCompositeProduct).RequireAuthorization();

            builder.MapPut(PostCompositeProduct).RequireAuthorization();

            builder.MapDelete(DeleteCompositeProduct).RequireAuthorization();

            builder.MapGet(GetCompositeProductById);

        }

        async Task<PagedResponse<CompositeProductDto>> GetPagedListCompositeProduct(IMediator mediator, [AsParameters] GetPagedListCompositeProductQuery model)
            => await mediator.Send<GetPagedListCompositeProductQuery, PagedResponse<CompositeProductDto>>(model);

        async Task<BaseResult<List<ProductForCompositeDto>>> GetProductsForComposite(IMediator mediator)
            => await mediator.Send<GetProductsForCompositeQuery, BaseResult<List<ProductForCompositeDto>>>(new GetProductsForCompositeQuery());

        async Task<BaseResult<CompositeProductDto>> GetCompositeProductById(IMediator mediator, [AsParameters] GetCompositeProductByIdQuery model)
       => await mediator.Send<GetCompositeProductByIdQuery, BaseResult<CompositeProductDto>>(model);

        async Task<BaseResult<long>> CreateCompositeProduct(IMediator mediator, [FromBody] CreateCompositeProductCommand model)
            => await mediator.Send<CreateCompositeProductCommand, BaseResult<long>>(model);

        async Task<BaseResult> UpdateCompositeProduct(IMediator mediator, [FromBody] UpdateCompositeProductCommand model)
            => await mediator.Send<UpdateCompositeProductCommand, BaseResult>(model);

        async Task<BaseResult> PostCompositeProduct(IMediator mediator, [FromBody] PostCompositeProductCommand model)
            => await mediator.Send<PostCompositeProductCommand, BaseResult>(model);

        async Task<BaseResult> DeleteCompositeProduct(IMediator mediator, [AsParameters] DeleteCompositeProductCommand model)
            => await mediator.Send<DeleteCompositeProductCommand, BaseResult>(model);
    }
}

