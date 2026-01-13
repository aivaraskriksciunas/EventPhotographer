using EventPhotographer.App.Events.Entities;
using EventPhotographer.App.Events.Resources;
using Riok.Mapperly.Abstractions;

namespace EventPhotographer.App.Events.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Source)]
public static partial class EventMapper
{
    public static partial Event ToEntity(this EventResource resource);
}
