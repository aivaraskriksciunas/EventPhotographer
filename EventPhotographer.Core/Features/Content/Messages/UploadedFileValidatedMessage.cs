namespace EventPhotographer.Core.Features.Content.Messages;

public class UploadedFileValidatedMessage
{
    public required Guid MediaFileId { get; init; }
}
