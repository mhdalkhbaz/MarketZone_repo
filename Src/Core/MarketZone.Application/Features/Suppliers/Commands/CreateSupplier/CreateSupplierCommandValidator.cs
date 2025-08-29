using FluentValidation;
using MarketZone.Application.Interfaces;

namespace MarketZone.Application.Features.Suppliers.Commands.CreateSupplier
{
    public class CreateSupplierCommandValidator : AbstractValidator<CreateSupplierCommand>
    {
        public CreateSupplierCommandValidator(ITranslator translator)
        {
            RuleFor(p => p.Name)
                .NotEmpty()
                .MaximumLength(100)
                .WithName(p => translator[nameof(p.Name)]);

            RuleFor(p => p.Phone)
                .MaximumLength(50)
                .WithName(p => translator[nameof(p.Phone)]);

            //RuleFor(p => p.WhatsAppPhone)
            //	.MaximumLength(50)
            //	.WithName(p => translator[nameof(p.WhatsAppPhone)]);

            //RuleFor(p => p.Email)
            //	.MaximumLength(100)
            //	.EmailAddress()
            //	.When(x => !string.IsNullOrWhiteSpace(x.Email))
            //	.WithName(p => translator[nameof(p.Email)]);

            RuleFor(p => p.Address)
                .MaximumLength(500)
                .WithName(p => translator[nameof(p.Address)]);
        }
    }
}



