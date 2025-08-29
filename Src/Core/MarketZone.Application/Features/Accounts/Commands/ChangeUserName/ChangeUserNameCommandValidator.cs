using FluentValidation;
using MarketZone.Application.Helpers;
using MarketZone.Application.Interfaces;

namespace MarketZone.Application.Features.Accounts.Commands.ChangeUserName
{
    public class ChangeUserNameCommandValidator : AbstractValidator<ChangeUserNameCommand>
    {
        public ChangeUserNameCommandValidator(ITranslator translator)
        {
            RuleFor(x => x.UserName)
                .NotEmpty()
                .MinimumLength(4)
                .Matches(Regexs.UserName)
                .WithName(p => translator[nameof(p.UserName)]);

        }
    }
}
