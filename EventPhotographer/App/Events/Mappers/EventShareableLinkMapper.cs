using EventPhotographer.App.Events.Entities;
using EventPhotographer.App.Events.Resources;
using Riok.Mapperly.Abstractions;

namespace EventPhotographer.App.Events.Mappers;

[Mapper]
public static partial class EventShareableLinkMapper
{
    public static partial EventShareableLinkResponseDto CreateResponseDto(EventShareableLink entity);
}
