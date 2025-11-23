using FluentValidation;
using MarketZone.Application.Interfaces;

namespace MarketZone.Application.Features.Products.Commands.UpdateCompositeProduct
{
    public class UpdateCompositeProductCommandValidator : AbstractValidator<UpdateCompositeProductCommand>
    {
        public UpdateCompositeProductCommandValidator(ITranslator translator)
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .GreaterThan(0)
                .WithName(x => translator[nameof(x.Id)]);

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
                .SetValidator(new UpdateCompositeProductDetailItemValidator(translator));
        }
    }

    public class UpdateCompositeProductDetailItemValidator : AbstractValidator<UpdateCompositeProductDetailItem>
    {
        public UpdateCompositeProductDetailItemValidator(ITranslator translator)
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

