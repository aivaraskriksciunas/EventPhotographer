using Riok.Mapperly.Abstractions;
using EventPhotographer.App.Events.DTO.Response;
using EventPhotographer.Core.Features.Events.Entities;

namespace EventPhotographer.App.Events.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
public static partial class EventShareableLinkMapper
{
    public static partial EventShareableLinkResponseDto CreateResponseDto(EventShareableLink entity);

    public static partial EventShareableLinkDetailResponseDto CreateDetailResponseDto(EventShareableLink entity);
}
