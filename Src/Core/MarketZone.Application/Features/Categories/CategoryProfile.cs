using AutoMapper;
using MarketZone.Application.Features.Categories.Commands.CreateCategory;
using MarketZone.Application.Features.Categories.Commands.UpdateCategory;
using MarketZone.Domain.Categories.DTOs;
using MarketZone.Domain.Categories.Entities;

namespace MarketZone.Application.Features.Categories
{
	public class CategoryProfile : Profile
	{
		public CategoryProfile()
		{
			CreateMap<Category, CategoryDto>()
				.ForMember(d => d.CreatedDateTime, o => o.MapFrom(s => s.Created));

			CreateMap<CreateCategoryCommand, Category>()
				.ConstructUsing(s => new Category(s.Name, s.Description));

			CreateMap<UpdateCategoryCommand, Category>()
				.ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
		}
	}
}


