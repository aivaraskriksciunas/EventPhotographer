using EventPhotographer.App.Users.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace EventPhotographer.App.Users.Dto;

public class RegisterRequestDtoValidator : AbstractValidator<RegisterRequestDto>
{
    public RegisterRequestDtoValidator(UserManager<User> userManager)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MustAsync(async (string email, CancellationToken cancellation) =>
            {
                var user = await userManager.FindByEmailAsync(email);

                return user == null;
            }).WithMessage("Account with this email address already exists");

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(100);
    }
}
