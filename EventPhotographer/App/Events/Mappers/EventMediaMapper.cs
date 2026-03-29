using EventPhotographer.App.Content.Entities;
using EventPhotographer.App.Events.DTO;
using Riok.Mapperly.Abstractions;

namespace EventPhotographer.App.Events.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
public partial class EventMediaMapper
{
    public static partial EventMediaResponseDto ToResponse(Media media);

    public static partial EventMediaResponseDto ToResponse(MediaFile media);

    public static partial IEnumerable<EventMediaResponseDto> ToResponse(IEnumerable<Media> media);
}
