using EventPhotographer.Core;
using EventPhotographer.Core.Features.Events.Entities;
using EventPhotographer.Core.Features.Events.Services;
using EventPhotographer.Core.Features.Users.Entities;
using EventPhotographer.UseCases.Common;
using EventPhotographer.UseCases.Common.Authorization;
using EventPhotographer.UseCases.Common.Commands;
using EventPhotographer.UseCases.Events.Authorization;
using FluentValidation;

namespace EventPhotographer.UseCases.Events;

public record CreateEvent : BaseEventEditCommand,
    IAuthorizationUserAware,
    IRequiresAuthorization
{
    public User User { get; set; } = null!;

    public IEnumerable<IAuthorizationRequirement> GetRequirements()
    {
        yield return new CreateEventRequirement();
    }
}

public class CreateEventValidator : AbstractValidator<CreateEvent>
{
    public CreateEventValidator()
    {
        Include(new BaseEventEditCommandValidator());
    }
}

public sealed class CreateEventHandler(
    EventService eventService,
    AppDbContext db)
    : ICommandHandler<CreateEvent, Event>
{

    public async Task<Result<Event>> HandleAsync(CreateEvent command, CancellationToken cancellationToken = default)
    {
        var entity = new Event
        {
            Name = command.Name,
            StartDate = command.StartDate ?? DateTime.UtcNow,
            UserId = command.User.Id,
            CreatedAt = DateTime.UtcNow,
        };

        var eventDuration = Enum.Parse<EventDuration>(command.EventDuration);
        entity.EndDate = eventService.CalculateEventEndDate(entity.StartDate, eventDuration);

        await db.Events.AddAsync(entity);
        await db.SaveChangesAsync();

        return entity;
    }
}