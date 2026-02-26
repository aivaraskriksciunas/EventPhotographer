using EventPhotographer.App.Events.DTO;
using FluentValidation;

namespace EventPhotographer.App.Events.Validators;

public class JoinEventRequestDtoValidator : AbstractValidator<JoinEventRequestDto>
{
    public JoinEventRequestDtoValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty();

        RuleFor(x => x.Name)
            .MinimumLength(3)
            .MaximumLength(100);
    }
}
