using MarketZone.Application.Features.Purchases.Commands.CreatePurchaseInvoice;
using MarketZone.Application.Features.Purchases.Commands.DeletePurchaseInvoice;
using MarketZone.Application.Features.Purchases.Commands.PostPurchaseInvoice;
using MarketZone.Application.Features.Purchases.Commands.UpdatePurchaseInvoice;
using MarketZone.Application.Features.Purchases.Queries.GetPagedListPurchaseInvoice;
using MarketZone.Application.Features.Purchases.Queries.GetPurchaseInvoiceById;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Purchases.DTOs;
using MarketZone.WebApi.Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Threading.Tasks;

namespace MarketZone.WebApi.Endpoints
{
	public class PurchaseInvoiceEndpoint : EndpointGroupBase
	{
		public override void Map(RouteGroupBuilder builder)
		{
			builder.MapGet(GetPagedListPurchaseInvoice);
			builder.MapGet(GetPurchaseInvoiceById);
			builder.MapPost(CreatePurchaseInvoice).RequireAuthorization();
			builder.MapPut(UpdatePurchaseInvoice).RequireAuthorization();
			builder.MapPut(PostPurchaseInvoice).RequireAuthorization();
			builder.MapDelete(DeletePurchaseInvoice).RequireAuthorization();
		}

		async Task<PagedResponse<PurchaseInvoiceDto>> GetPagedListPurchaseInvoice(IMediator mediator, [AsParameters] GetPagedListPurchaseInvoiceQuery model)
			=> await mediator.Send<GetPagedListPurchaseInvoiceQuery, PagedResponse<PurchaseInvoiceDto>>(model);

		async Task<BaseResult<PurchaseInvoiceDto>> GetPurchaseInvoiceById(IMediator mediator, [AsParameters] GetPurchaseInvoiceByIdQuery model)
			=> await mediator.Send<GetPurchaseInvoiceByIdQuery, BaseResult<PurchaseInvoiceDto>>(model);

		async Task<BaseResult<long>> CreatePurchaseInvoice(IMediator mediator, [FromBody] CreatePurchaseInvoiceCommand model)
			=> await mediator.Send<CreatePurchaseInvoiceCommand, BaseResult<long>>(model);

		async Task<BaseResult> UpdatePurchaseInvoice(IMediator mediator, [FromBody] UpdatePurchaseInvoiceCommand model)
			=> await mediator.Send<UpdatePurchaseInvoiceCommand, BaseResult>(model);

		async Task<BaseResult> PostPurchaseInvoice(IMediator mediator, PostPurchaseInvoiceCommand model)
			=> await mediator.Send<PostPurchaseInvoiceCommand, BaseResult>(model);

		async Task<BaseResult> DeletePurchaseInvoice(IMediator mediator, [AsParameters] DeletePurchaseInvoiceCommand model)
			=> await mediator.Send<DeletePurchaseInvoiceCommand, BaseResult>(model);
	}
}



