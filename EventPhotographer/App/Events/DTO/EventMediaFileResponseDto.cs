namespace EventPhotographer.App.Events.DTO;

public class EventMediaFileResponseDto
{
    public Guid Id { get; set; }

    public string? MimeType { get; set; }

    public ulong FileSize { get; set; }
}
