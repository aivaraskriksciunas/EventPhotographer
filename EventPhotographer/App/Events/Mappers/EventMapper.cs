using EventPhotographer.App.Events.Entities;
using EventPhotographer.App.Events.DTO;
using EventPhotographer.App.Users.Entities;
using Riok.Mapperly.Abstractions;

namespace EventPhotographer.App.Events.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
public static partial class EventMapper
{
    [MapProperty(nameof(EventDto.StartDate), nameof(Event.StartDate), Use = nameof(MapStartDate))]
    [MapperIgnoreSource(nameof(EventDto.EventDuration))]
    public static partial Event ToEntity(this EventDto resource, User user);

    [MapProperty(nameof(EventDto.StartDate), nameof(Event.StartDate), Use = nameof(MapStartDate))]
    [MapperIgnoreSource(nameof(EventDto.EventDuration))]
    public static partial void UpdateFromDto([MappingTarget]this Event entity, EventDto resource);

    public static partial EventResponseDto CreateResponseDto(Event entity);

    public static partial IEnumerable<EventResponseDto> CreateResponseDtos(this IEnumerable<Event> entities);

    public static partial ParticipantResponseDto CreateResponseDto(Participant entity);

    [UserMapping(Default = false)]
    private static DateTime MapStartDate(DateTime? startDate)
    {
        return startDate ?? DateTime.UtcNow;
    }
}
