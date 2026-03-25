namespace EventPhotographer.App.Content.DTO;

public class MediaFileResponseDto
{
    public Guid Id { get; set; }

    public required string MimeType { get; set; }

    public uint FileSize { get; set; }
}
