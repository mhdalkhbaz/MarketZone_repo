using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketZone.Application.DTOs;
using MarketZone.Application.Features.Roasting.Commands.CreateRoast;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using MarketZone.Infrastructure.Persistence.Contexts;
using MarketZone.WebApi.Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace MarketZone.WebApi.Endpoints
{
	public class RoastingEndpoint : EndpointGroupBase
	{
		public override void Map(RouteGroupBuilder builder)
		{
			builder.MapPost(CreateRoast).RequireAuthorization();
		}

		async Task<BaseResult<long>> CreateRoast(IMediator mediator, CreateRoastCommand model)
			=> await mediator.Send<CreateRoastCommand, BaseResult<long>>(model);
	}
}


