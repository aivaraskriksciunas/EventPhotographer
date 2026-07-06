using EventPhotographer.Core.Features.Events.Entities;
using EventPhotographer.UseCases.Common.Commands;
using FluentValidation;

namespace EventPhotographer.UseCases.Events;

public record BaseEventEditCommand : ICommand<Event>
{
    public string Name { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; }
    public string EventDuration { get; set; } = string.Empty;
}

internal class BaseEventEditCommandValidator : AbstractValidator<BaseEventEditCommand>
{
    public BaseEventEditCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100)
            .MinimumLength(3);

        RuleFor(x => x.EventDuration)
            .IsEnumName(typeof(EventDuration));

        When(x => x.StartDate.HasValue, () =>
        {
            RuleFor(x => x.StartDate)
                .GreaterThan(DateTime.UtcNow.AddMinutes(-1))
                .LessThan(DateTime.UtcNow.AddMonths(6))
                .WithMessage("Date must not be more than 6 months in the future");
        });
    }
}