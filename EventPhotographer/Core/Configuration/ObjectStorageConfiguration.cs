using Amazon.Extensions.NETCore.Setup;

namespace EventPhotographer.Core.Configuration;

public class ObjectStorageConfiguration
{
    public required string ServiceURL { get; set; }

    public required string BucketName { get; set; }

    public required string AccessKey { get; set; }

    public required string Password { get; set; }

    public bool ForcePathStyle { get; set; } = false;
}
