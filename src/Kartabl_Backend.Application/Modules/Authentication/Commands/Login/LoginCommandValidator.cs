using FluentValidation;
using Kartabl_Backend.Application.Modules.Authentication.Commands.Login;

namespace Kartabl_Backend.Application.Modules.Authentication.Commands.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.NationalCode)
            .NotEmpty()
            .WithMessage("National code is required.")
            .Length(10)
            .WithMessage("National code must be exactly 10 digits.")
            .Matches(@"^\d+$")
            .WithMessage("National code must contain digits only.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(9)
            .WithMessage("Password must be at least 9 characters.");
    }
}