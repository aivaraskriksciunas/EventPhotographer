namespace EventPhotographer.Core.Features.Content.Messages;

public record ValidateUploadedFileMessage
{
    public required Guid MediaFileId { get; init; }

    public uint DelayValidationMs { get; init; } = 0;
}
