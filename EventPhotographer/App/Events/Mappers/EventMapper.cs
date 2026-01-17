using EventPhotographer.App.Events.Entities;
using EventPhotographer.App.Events.Resources;
using Riok.Mapperly.Abstractions;

namespace EventPhotographer.App.Events.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Source)]
public static partial class EventMapper
{
    [MapProperty(nameof(EventDto.StartDate), nameof(Event.StartDate), Use = nameof(MapStartDate))]
    [MapperIgnoreSource(nameof(EventDto.EventDuration))]
    public static partial Event ToEntity(this EventDto resource);

    [MapProperty(nameof(EventDto.StartDate), nameof(Event.StartDate), Use = nameof(MapStartDate))]
    [MapperIgnoreSource(nameof(EventDto.EventDuration))]
    public static partial void UpdateFromDto([MappingTarget]this Event entity, EventDto resource);

    [UserMapping(Default = false)]
    private static DateTime MapStartDate(DateTime? startDate)
    {
        return startDate ?? DateTime.UtcNow;
    }
}
