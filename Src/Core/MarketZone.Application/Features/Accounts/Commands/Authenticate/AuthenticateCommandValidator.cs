using FluentValidation;
using MarketZone.Application.Interfaces;

namespace MarketZone.Application.Features.Accounts.Commands.Authenticate
{
    public class AuthenticateCommandValidator : AbstractValidator<AuthenticateCommand>
    {
        public AuthenticateCommandValidator(ITranslator translator)
        {
            RuleFor(x => x.UserName)
                .NotEmpty()
                .WithName(p => translator[nameof(p.UserName)]);

            RuleFor(x => x.Password)
                .NotEmpty()
                //.Matches(Regexs.Password)
                .WithName(p => translator[nameof(p.Password)]);
        }
    }
}