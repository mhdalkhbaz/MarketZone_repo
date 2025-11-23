using FluentValidation;
using MarketZone.Application.Interfaces;

namespace MarketZone.Application.Features.Products.Commands.PostCompositeProduct
{
    public class PostCompositeProductCommandValidator : AbstractValidator<PostCompositeProductCommand>
    {
        public PostCompositeProductCommandValidator(ITranslator translator)
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .GreaterThan(0)
                .WithName(x => translator[nameof(x.Id)]);
        }
    }
}

