namespace EventPhotographer.App.Content.DTO;

public class MediaResponseDto
{
    public Guid? UploadToken { get; set; }

    public DateTime CreatedAt { get; set; }

    public IEnumerable<MediaFileResponseDto> Files { get; set; } = new List<MediaFileResponseDto>();
}
