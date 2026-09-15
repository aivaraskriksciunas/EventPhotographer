namespace EventPhotographer.Core.Features.Content.Messages;

public class UploadedFileThumbnailGeneratedMessage
{
    public Guid MediaId { get; set; }
    public Guid ThumbnailFileId { get; set; }
}
