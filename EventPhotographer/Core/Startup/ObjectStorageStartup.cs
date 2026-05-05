using Amazon.S3;
using EventPhotographer.Core.Configuration;
using Microsoft.Extensions.Options;

namespace EventPhotographer.Core.Startup;

public class ObjectStorageStartup : IHostedService
{
    private readonly IOptions<ObjectStorageConfiguration> _options;
    private readonly IAmazonS3 _s3Client;
    private readonly ILogger<ObjectStorageStartup> _logger;

    public ObjectStorageStartup(
        IAmazonS3 s3,
        IOptions<ObjectStorageConfiguration> objectStorageConfiguration,
        ILogger<ObjectStorageStartup> logger)
    {
        _options = objectStorageConfiguration;
        _s3Client = s3;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Ensuring object storage bucket exists...");
        await _s3Client.EnsureBucketExistsAsync(_options.Value.BucketName);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
