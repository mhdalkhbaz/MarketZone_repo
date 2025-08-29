using System.Threading.Tasks;
using MarketZone.Application.Features.Sales.Commands.CreateSalesInvoice;
using MarketZone.Application.Features.Sales.Commands.DeleteSalesInvoice;
using MarketZone.Application.Features.Sales.Commands.UpdateSalesInvoice;
using MarketZone.Application.Features.Sales.Commands.PostSalesInvoice;
using MarketZone.Application.Features.Sales.Queries.GetPagedListSalesInvoice;
using MarketZone.Application.Features.Sales.Queries.GetSalesInvoiceById;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Sales.DTOs;
using MarketZone.WebApi.Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace MarketZone.WebApi.Endpoints
{
	public class SalesInvoiceEndpoint : EndpointGroupBase
	{
		public override void Map(RouteGroupBuilder builder)
		{
			builder.MapGet(GetPagedListSalesInvoice);
			builder.MapGet(GetSalesInvoiceById);
			builder.MapPost(CreateSalesInvoice).RequireAuthorization();
			builder.MapPut(UpdateSalesInvoice).RequireAuthorization();
			builder.MapPut(PostSalesInvoice).RequireAuthorization();
			builder.MapDelete(DeleteSalesInvoice).RequireAuthorization();
		}

		async Task<PagedResponse<SalesInvoiceDto>> GetPagedListSalesInvoice(IMediator mediator, [AsParameters] GetPagedListSalesInvoiceQuery model)
			=> await mediator.Send<GetPagedListSalesInvoiceQuery, PagedResponse<SalesInvoiceDto>>(model);

		async Task<BaseResult<SalesInvoiceDto>> GetSalesInvoiceById(IMediator mediator, [AsParameters] GetSalesInvoiceByIdQuery model)
			=> await mediator.Send<GetSalesInvoiceByIdQuery, BaseResult<SalesInvoiceDto>>(model);

		async Task<BaseResult<long>> CreateSalesInvoice(IMediator mediator, CreateSalesInvoiceCommand model)
			=> await mediator.Send<CreateSalesInvoiceCommand, BaseResult<long>>(model);

		async Task<BaseResult> UpdateSalesInvoice(IMediator mediator, UpdateSalesInvoiceCommand model)
			=> await mediator.Send<UpdateSalesInvoiceCommand, BaseResult>(model);

		async Task<BaseResult> PostSalesInvoice(IMediator mediator, PostSalesInvoiceCommand model)
			=> await mediator.Send<PostSalesInvoiceCommand, BaseResult>(model);

		async Task<BaseResult> DeleteSalesInvoice(IMediator mediator, [AsParameters] DeleteSalesInvoiceCommand model)
			=> await mediator.Send<DeleteSalesInvoiceCommand, BaseResult>(model);
	}
}



