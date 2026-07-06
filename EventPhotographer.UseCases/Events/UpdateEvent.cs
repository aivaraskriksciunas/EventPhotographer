using EventPhotographer.Core;
using EventPhotographer.Core.Features.Events.Entities;
using EventPhotographer.Core.Features.Events.Services;
using EventPhotographer.Core.Features.Users.Entities;
using EventPhotographer.Core.Util;
using EventPhotographer.UseCases.Common;
using EventPhotographer.UseCases.Common.Authorization;
using EventPhotographer.UseCases.Common.Commands;
using EventPhotographer.UseCases.Events.Authorization;
using FluentValidation;

namespace EventPhotographer.UseCases.Events;

public record UpdateEvent : BaseEventEditCommand,
    IRequiresResourceAuthorization,
    IAuthorizationUserAware
{
    public Event Event { get; set; } = null!;
    public User User { get; set; } = null!;

    public IEntity GetAuthorizationResource() => Event;

    public IEnumerable<IAuthorizationRequirement> GetRequirements()
    {
        yield return new ManageEventRequirement();
    }
}

public class UpdateEventValidator : AbstractValidator<UpdateEvent>
{
    public UpdateEventValidator()
    {
        Include(new BaseEventEditCommandValidator());
    }
}

public sealed class UpdateEventHandler(
    EventService eventService,
    AppDbContext db)
    : ICommandHandler<UpdateEvent, Event>
{
    public IEnumerable<IAuthorizationRequirement> Requirements => throw new NotImplementedException();

    public async Task<Result<Event>> HandleAsync(UpdateEvent command, CancellationToken cancellationToken = default)
    {
        var entity = command.Event;
        var eventDuration = Enum.Parse<EventDuration>(command.EventDuration);

        entity.Name = command.Name;
        entity.StartDate = command.StartDate ?? entity.StartDate;
        entity.EndDate = eventService.CalculateEventEndDate(entity.StartDate, eventDuration);
        await db.SaveChangesAsync();

        return entity;
    }
}