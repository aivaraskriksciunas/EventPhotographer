using EventPhotographer.App.Content.DTO;
using EventPhotographer.Data.Entities.Content;
using Riok.Mapperly.Abstractions;

namespace EventPhotographer.App.Content.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
public partial class MediaMapper
{
    public static partial MediaResponseDto ToResponse(Media media);

    public static partial IEnumerable<MediaResponseDto> ToResponse(IEnumerable<Media> media);
}
