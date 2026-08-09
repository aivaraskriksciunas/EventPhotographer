namespace EventPhotographer.App.Content.DTO;

public record CreateMediaResponseDto
{
    public required MediaResponseDto Media { get; set; }

    public required string UploadUrl { get; set; }

    public required Dictionary<string, string> Fields { get; set; }
}
