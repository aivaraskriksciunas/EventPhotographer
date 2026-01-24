using FluentValidation;

namespace EventPhotographer.App.Users.Dto;

public class LoginRequestDtoValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestDtoValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress()
            .NotEmpty();

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}
