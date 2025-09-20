using FluentValidation;
using MarketZone.Application.Interfaces;

namespace MarketZone.Application.Features.Employees.Commands.CreateEmployee
{
    public class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand>
    {
        public CreateEmployeeCommandValidator(ITranslator translator)
        {
            RuleFor(p => p.FirstName)
                .NotEmpty()
                .MaximumLength(100)
                .WithName(p => translator[nameof(p.FirstName)]);

            RuleFor(p => p.LastName)
                .NotEmpty()
                .MaximumLength(100)
                .WithName(p => translator[nameof(p.LastName)]);

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

            //RuleFor(p => p.Address)
            //    .MaximumLength(500)
            //    .WithName(p => translator[nameof(p.Address)]);

            RuleFor(p => p.JobTitle)
                .MaximumLength(100)
                .WithName(p => translator[nameof(p.JobTitle)]);

            //RuleFor(p => p.Salary)
            //    .NotNull()
            //    .GreaterThanOrEqualTo(0)
            //    .WithName(p => translator[nameof(p.Salary)]);

            //RuleFor(p => p.HireDate)
            //    .NotEmpty()
            //    .WithName(p => translator[nameof(p.HireDate)]);
        }
    }
}



