using EventPhotographer.Core.Features.Content.Entities;

namespace EventPhotographer.App.Content.DTO;

public class MediaResponseDto
{
    public Guid? Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public MediaStatus Status { get; set;}

    public IEnumerable<MediaFileResponseDto> Files { get; set; } = new List<MediaFileResponseDto>();
}
