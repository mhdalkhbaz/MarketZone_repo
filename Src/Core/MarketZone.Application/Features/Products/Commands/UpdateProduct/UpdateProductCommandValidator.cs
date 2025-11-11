using FluentValidation;
using MarketZone.Application.Interfaces;

namespace MarketZone.Application.Features.Products.Commands.UpdateProduct
{
    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator(ITranslator translator)
        {

            RuleFor(p => p.Name)
                .NotEmpty()
                .MaximumLength(100)
                .WithName(p => translator[nameof(p.Name)]);

            RuleFor(p => p.CommissionPerKg)
                .GreaterThanOrEqualTo(0m)
                .When(p => p.CommissionPerKg.HasValue)
                .WithName(p => translator[nameof(p.CommissionPerKg)]);

            //RuleFor(x => x.BarCode)
            //    .NotEmpty()
            //    .MaximumLength(50)
            //    .WithName(p => translator[nameof(p.BarCode)]);

            //RuleFor(x => x.CategoryId)
            //    .NotEmpty()
            //    .WithName(p => translator[nameof(p.CategoryId)]);

            //RuleFor(x => x.Description)
            //    .MaximumLength(500)
            //    .WithName(p => translator[nameof(p.Description)]);

            //RuleFor(x => x.UnitOfMeasure)
            //    .MaximumLength(100)
            //    .WithName(p => translator[nameof(p.UnitOfMeasure)]);  
        }
    }

}
