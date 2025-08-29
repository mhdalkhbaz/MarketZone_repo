using System.Threading.Tasks;
using MarketZone.Application.Features.Categories.Commands.CreateCategory;
using MarketZone.Application.Features.Categories.Commands.DeleteCategory;
using MarketZone.Application.Features.Categories.Commands.UpdateCategory;
using MarketZone.Application.Features.Categories.Queries.GetCategoryById;
using MarketZone.Application.Features.Categories.Queries.GetPagedListCategory;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Categories.DTOs;
using MarketZone.WebApi.Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace MarketZone.WebApi.Endpoints
{
	public class CategoryEndpoint : EndpointGroupBase
	{
		public override void Map(RouteGroupBuilder builder)
		{
			builder.MapGet(GetPagedListCategory);
			builder.MapGet(GetCategoryById);
			builder.MapPost(CreateCategory).RequireAuthorization();
			builder.MapPut(UpdateCategory).RequireAuthorization();
			builder.MapDelete(DeleteCategory).RequireAuthorization();
		}

		async Task<PagedResponse<CategoryDto>> GetPagedListCategory(IMediator mediator, [AsParameters] GetPagedListCategoryQuery model)
			=> await mediator.Send<GetPagedListCategoryQuery, PagedResponse<CategoryDto>>(model);

		async Task<BaseResult<CategoryDto>> GetCategoryById(IMediator mediator, [AsParameters] GetCategoryByIdQuery model)
			=> await mediator.Send<GetCategoryByIdQuery, BaseResult<CategoryDto>>(model);

		async Task<BaseResult<long>> CreateCategory(IMediator mediator, CreateCategoryCommand model)
			=> await mediator.Send<CreateCategoryCommand, BaseResult<long>>(model);

		async Task<BaseResult> UpdateCategory(IMediator mediator, UpdateCategoryCommand model)
			=> await mediator.Send<UpdateCategoryCommand, BaseResult>(model);

		async Task<BaseResult> DeleteCategory(IMediator mediator, [AsParameters] DeleteCategoryCommand model)
			=> await mediator.Send<DeleteCategoryCommand, BaseResult>(model);
	}
}


