using FluentValidation;

namespace EventPhotographer.App.Events.Resources;

public class EventDtoValidator : AbstractValidator<EventDto>
{
    public EventDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100)
            .MinimumLength(3);

        RuleFor(x => x.EventDuration)
            .IsEnumName(typeof(EventDuration));

        When(x => x.StartDate.HasValue, () =>
        {
            RuleFor(x => x.StartDate!.Value)
                .GreaterThan(DateTime.UtcNow.AddMinutes(-1))
                .LessThan(DateTime.UtcNow.AddDays(7))
                .WithMessage("Date must not be more than 1 week in the future");
        });
    }
}
