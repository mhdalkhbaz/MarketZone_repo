using FluentValidation;
using MarketZone.Application.Interfaces;

namespace MarketZone.Application.Features.Products.Commands.CreateCompositeProduct
{
    public class CreateCompositeProductCommandValidator : AbstractValidator<CreateCompositeProductCommand>
    {
        public CreateCompositeProductCommandValidator(ITranslator translator)
        {
            RuleFor(x => x.ResultingProductId)
                .NotEmpty()
                .GreaterThan(0)
                .WithName(x => translator[nameof(x.ResultingProductId)]);

            RuleFor(x => x.SalePrice)
                .GreaterThanOrEqualTo(0)
                .WithName(x => translator[nameof(x.SalePrice)]);

            RuleFor(x => x.CommissionPerKg)
                .GreaterThanOrEqualTo(0)
                .WithName(x => translator[nameof(x.CommissionPerKg)]);

            RuleFor(x => x.Details)
                .NotEmpty()
                .WithMessage(translator.GetString("Composite_Product_Must_Have_Details"))
                .WithName(x => translator[nameof(x.Details)]);

            RuleForEach(x => x.Details)
                .SetValidator(new CreateCompositeProductDetailItemValidator(translator));
        }
    }

    public class CreateCompositeProductDetailItemValidator : AbstractValidator<CreateCompositeProductDetailItem>
    {
        public CreateCompositeProductDetailItemValidator(ITranslator translator)
        {
            RuleFor(x => x.ComponentProductId)
                .NotEmpty()
                .GreaterThan(0)
                .WithName(x => translator[nameof(x.ComponentProductId)]);

            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithName(x => translator[nameof(x.Quantity)]);
        }
    }
}

