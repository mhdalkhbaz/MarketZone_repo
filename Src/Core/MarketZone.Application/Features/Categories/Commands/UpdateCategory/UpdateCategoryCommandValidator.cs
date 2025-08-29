using FluentValidation;
using MarketZone.Application.Interfaces;

namespace MarketZone.Application.Features.Categories.Commands.UpdateCategory
{
	public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
	{
		public UpdateCategoryCommandValidator(ITranslator translator)
		{
			RuleFor(p => p.Name)
				.NotEmpty()
				.MaximumLength(100)
				.WithName(p => translator[nameof(p.Name)]);

			RuleFor(x => x.Description)
				.MaximumLength(500)
				.WithName(p => translator[nameof(p.Description)]);
		}
	}
}


