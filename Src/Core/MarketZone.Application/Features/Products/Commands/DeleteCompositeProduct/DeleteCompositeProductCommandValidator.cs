using FluentValidation;
using MarketZone.Application.Interfaces;

namespace MarketZone.Application.Features.Products.Commands.DeleteCompositeProduct
{
    public class DeleteCompositeProductCommandValidator : AbstractValidator<DeleteCompositeProductCommand>
    {
        public DeleteCompositeProductCommandValidator(ITranslator translator)
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .GreaterThan(0)
                .WithName(x => translator[nameof(x.Id)]);
        }
    }
}

