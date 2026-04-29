using EventPhotographer.Data.Entities.Events;
using Riok.Mapperly.Abstractions;
using EventPhotographer.App.Events.DTO.Response;

namespace EventPhotographer.App.Events.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
public static partial class EventShareableLinkMapper
{
    public static partial EventShareableLinkResponseDto CreateResponseDto(EventShareableLink entity);
}
